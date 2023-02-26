﻿using System.Buffers.Binary;
using Ezsp.Ash;

namespace Ezsp;

public class EzspClient
{
    private readonly AshClient client;
    private byte index;
    private byte version;
    private TaskCompletionSource<byte[]>?[] tcss = new TaskCompletionSource<byte[]>[256];

    public EzspClient(Stream stream)
    {
        client = new AshClient(stream)
        {
            DataReceived = DataReceived
        };
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await client.ConnectAsync(cancellationToken);
        var response = await SendAsync(EzspCommand.Version, 4);
        version = response[3];
        await SendAsync(EzspCommand.Version, version);
    }

    public async Task<byte[]> SendAsync(EzspCommand cmd, params byte[] data)
    {
        var tcs = new TaskCompletionSource<byte[]>();
        tcss[index] = tcs;

        var request = new byte[data.Length + (version > 4 ? 5 : 3)];
        var i = 0;
        request[i++] = index++;
        if (version >= 8)
        {
            request[i++] = 0;
            request[i++] = 1;
            BinaryPrimitives.WriteUInt16BigEndian(request.AsSpan(i), (ushort)cmd);
            i += 2;
        }
        else if (version >= 6)
        {
            request[i++] = 0;
            request[i++] = 0xFF;
            request[i++] = 0;
            request[i++] = (byte)cmd;
        }
        else
        {
            request[i++] = 0;
            request[i++] = (byte)cmd;
        }
  
        data.CopyTo(request.AsSpan(i));
        await client.SendAsync(request);
        return await tcs.Task;
    }

    private void DataReceived(byte[] data)
    {
        var i = data[0];
        var cts = tcss[i];
        tcss[i] = null;
        cts?.SetResult(data);
    }
}