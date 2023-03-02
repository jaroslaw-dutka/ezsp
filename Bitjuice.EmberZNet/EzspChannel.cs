using System.Buffers.Binary;
using Bitjuice.EmberZNet.Ash;

namespace Bitjuice.EmberZNet;

public class EzspChannel
{
    private readonly AshDuplexChannel channel;
    private byte msgIndex;
    private byte version;
    private TaskCompletionSource<ReadOnlyMemory<byte>>?[] tcss = new TaskCompletionSource<ReadOnlyMemory<byte>>[256];

    public Action<byte[]>? CallbackReceived { get; set; }

    public EzspChannel(Stream stream)
    {
        channel = new AshDuplexChannel(stream)
        {
            DataReceived = DataReceived
        };
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await DisconnectAsync(cancellationToken);
        await channel.ConnectAsync(cancellationToken);
        var response = await SendAsync(EzspCommand.Version, 4);
        version = response.Span[0];
        await SendAsync(EzspCommand.Version, version);
    }

    public async Task DisconnectAsync(CancellationToken cancellationToken)
    {
        await channel.DisconnectAsync(cancellationToken);

        msgIndex = 0;
        version = 0;
        for (var i = 0; i < tcss.Length; i++)
        {
            var tcs = tcss[i];
            tcss[i] = null;
            if (tcs is not null)
                tcs.SetResult(Array.Empty<byte>());
        }
    }

    public async Task<TResponse> SendAsync<TResponse>(EzspCommand cmd)
    {
        var responseBytes = await SendAsync(cmd, Array.Empty<byte>());
        return EzspSerializer.Deserialize<TResponse>(responseBytes.Span.ToArray());
    }

    public async Task<TResponse> SendAsync<TRequest, TResponse>(EzspCommand cmd, TRequest request)
    {
        var requestBytes = EzspSerializer.Serialize(request);
        var responseBytes = await SendAsync(cmd, requestBytes);
        return EzspSerializer.Deserialize<TResponse>(responseBytes.Span.ToArray());
    }

    public Task<ReadOnlyMemory<byte>> SendAsync(EzspCommand cmd, params byte[] data)
    {
        var tcs = new TaskCompletionSource<ReadOnlyMemory<byte>>();
        tcss[msgIndex] = tcs;

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
        channel.AddToQueue(request);
        return tcs.Task;
    }

    private void DataReceived(byte[] data)
    {
        var i = data[0];
        var cts = tcss[i];
        tcss[i] = null;
        var memory = data.AsMemory(version > 4 ? 5 : 3);
        if (cts is not null)
            cts.SetResult(memory);
        else
            CallbackReceived?.Invoke(data);
    }
}