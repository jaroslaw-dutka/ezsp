using System.Buffers.Binary;
using Ezsp.Ash;

namespace Ezsp;

public class EzspClient
{
    private readonly AshClient client;
    private byte index;
    private byte version;

    public EzspClient(Stream stream)
    {
        client = new AshClient(stream);
    }

    public async Task ConnectAsync()
    {
        client.Connect();
        var response = await SendAsync(EzspCommand.Version, 4);
        version = response[0];
        await SendAsync(EzspCommand.Version, version);
    }

    public async Task<byte[]> SendAsync(EzspCommand cmd, params byte[] data)
    {
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
        var response = await client.SendSync(request);
        if (response is null)
            return null;
        return response.AsSpan(i).ToArray();
    }
}