using Ezsp.Ash;

namespace Ezsp;

public class EzspClient
{
    private readonly AshClient client;
    private byte index;

    public EzspClient(AshClient client)
    {
        this.client = client;
    }

    public void Reset()
    {
        client.Reset();
        client.Write(new AshFrame { Control = new AshControl { Type = AshFrameType.Rst } });
        Read();
        SendLegacy(EzspCommand.Version, 4);
        var response = Read();
    }

    public void SendLegacy(EzspCommand cmd, params byte[] parameters)
    {
        var data = new byte[1 + 1 + 1 + parameters.Length];
        data[0] = index;
        data[1] = 0;
        data[2] = (byte)cmd;
        parameters.CopyTo(data, 3);

        client.Write(new AshFrame
        {
            Control = new AshControl
            {
                Type = AshFrameType.Data,
                FrameNumber = index,
                AckNumber = index,
            },
            Data = data
        });

        index++;
    }

    public void Send(EzspCommand cmd, params byte[] parameters)
    {
        var cmdBytes = BitConverter.GetBytes((ushort)cmd);

        var data = new byte[1 + 2 + 2 + parameters.Length];
        data[0] = index;
        data[1] = 0;
        data[2] = 1;
        data[3] = cmdBytes[0];
        data[4] = cmdBytes[1];
        parameters.CopyTo(data, 5);

        client.Write(new AshFrame
        {
            Control = new AshControl
            {
                Type = AshFrameType.Data,
                FrameNumber = index,
                AckNumber = index,
            },
            Data = data
        });

        index++;
    }

    public byte[] Read()
    {
        var frame = client.Read();
        if (frame.Control.Type == AshFrameType.Data)
        {
            client.Write(new AshFrame
            {
                Control = new AshControl
                {
                    Type = AshFrameType.Ack, 
                    AckNumber = frame.Control.AckNumber
                }
            });
        }
        return frame.Data;
    }
}