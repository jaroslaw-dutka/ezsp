using System.Buffers.Binary;
using Ezsp.Ash;

namespace Ezsp;

public class EzspClientV2
{
    private readonly AshController client;
    private byte index;
    private byte version;

    public EzspClientV2(AshController client)
    {
        this.client = client;
    }

    public async Task ConnectAsync()
    {
        client.Connect();
        var response = await SendAsync(EzspCommand.Version, 4);
        version = response[3];
        await SendAsync(EzspCommand.Version, version);
    }

    public async Task<byte[]> SendAsync(EzspCommand cmd, params byte[] data)
    {
        var buffer = new byte[data.Length + (version > 4 ? 5 : 3)];
        var i = 0;
        buffer[i++] = index++;
        if (version >= 8)
        {
            buffer[i++] = 0;
            buffer[i++] = 1;
            BinaryPrimitives.WriteUInt16BigEndian(buffer.AsSpan(i), (ushort)cmd);
            i += 2;
        }
        else if (version >= 6)
        {
            buffer[i++] = 0;
            buffer[i++] = 0xFF;
            buffer[i++] = 0;
            buffer[i++] = (byte)cmd;
        }
        else
        {
            buffer[i++] = 0;
            buffer[i++] = (byte)cmd;
        }
  
        data.CopyTo(buffer.AsSpan(i));
        return await client.SendSync(buffer);
    }
}