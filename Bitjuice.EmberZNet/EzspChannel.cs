using System.Buffers.Binary;
using System.Collections.Concurrent;
using Bitjuice.EmberZNet.Ash;

namespace Bitjuice.EmberZNet;

public class EzspChannel: IAshDataHandler, IEzspChannel
{
    private readonly IAshChannel channel;
    private readonly ConcurrentDictionary<byte, TaskCompletionSource<byte[]>> receiveQueue = new();
    private IEzspCallbackHandler handler;
    private byte msgIndex;
    private byte version;
    
    public EzspChannel(IAshChannel channel)
    {
        this.channel = channel;
    }

    public EzspChannel(Stream stream): this(new AshChannel(stream))
    {
    }

    public async Task ConnectAsync(IEzspCallbackHandler handler, CancellationToken cancellationToken)
    {
        this.handler = handler;

        await channel.ConnectAsync(this, cancellationToken);

        var response = await SendAsync(EzspCommand.Version, 4);
        version = response[0];
        await SendAsync(EzspCommand.Version, version);
    }

    public async Task DisconnectAsync()
    {
        await channel.DisconnectAsync();

        msgIndex = 0;
        version = 0;
        foreach (var tcs in receiveQueue.Values) 
            tcs.SetResult(Array.Empty<byte>());
        receiveQueue.Clear();
    }

    public async Task<TResponse> SendAsync<TResponse>(EzspCommand cmd)
    {
        var responseBytes = await SendAsync(cmd, Array.Empty<byte>());
        return EzspSerializer.Deserialize<TResponse>(responseBytes);
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(EzspCommand cmd, TRequest request)
    {
        var requestBytes = EzspSerializer.Serialize(request);
        var responseBytes = await SendAsync(cmd, requestBytes);
        return EzspSerializer.Deserialize<TResponse>(responseBytes);
    }

    public Task<byte[]> SendAsync(EzspCommand cmd, params byte[] data)
    {
        var tcs = new TaskCompletionSource<byte[]>();
        receiveQueue[msgIndex] = tcs;

        var request = new byte[data.Length + (version > 4 ? 5 : 3)];
        var i = 0;

        request[i++] = msgIndex++;
        request[i++] = 0;

        if (version >= 8)
        {
            request[i++] = 1;
            BinaryPrimitives.WriteUInt16BigEndian(request.AsSpan(i), (ushort)cmd);
            i += 2;
        }
        else if (version >= 6)
        {
            request[i++] = 0xFF;
            request[i++] = 0;
            request[i++] = (byte)cmd;
        }
        else
        {
            request[i++] = (byte)cmd;
        }

        data.CopyTo(request.AsSpan(i));
        channel.Send(request);
        return tcs.Task;
    }

    public async Task HandleAsync(ReadOnlyMemory<byte> data)
    {
        if (receiveQueue.TryRemove(data.Span[0], out var tcs))
            tcs.SetResult(data.Slice(version > 4 ? 5 : 3).ToArray());
        else
            await handler.HandleCallbackAsync(data);
    }
}