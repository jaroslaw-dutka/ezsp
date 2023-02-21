using System.Net.Sockets;
using XiaomiGateway3;
using XiaomiGateway3.Ash;

var tcp = new TcpClient();
tcp.Connect("192.168.1.40", 8888);
using var stream = tcp.GetStream();
var ash = new AshClient(stream);
var ezsp = new EzspClient(ash);

byte index = 0;

// Reset
ash.Reset();
ash.Write(new AshFrame
{
    Control = new AshControl { Type = AshFrameType.Rst }
});
HandleResponse();

// GetVersionLegacy
ash.Write(new AshFrame
{
    Control = new AshControl { Type = AshFrameType.Data, FrameNumber = index, AckNumber = index },
    Data = new byte[] { index, 0, 0, 4 }
});
HandleResponse();
index++;

// GetVersion
ash.Write(new AshFrame
{
    Control = new AshControl { Type = AshFrameType.Data, FrameNumber = index, AckNumber = index },
    Data = new byte[] { index, 0, 1, 0, 0, 8 }
});
HandleResponse();
index++;

// NOP
// ash.Write(new AshFrame
// {
//     Control = new AshControl { Type = AshFrameType.Data, FrameNumber = index, AckNumber = index },
//     Data = new byte[] { index, 0, 0, 0, 5 }
// });
// HandleResponse();
// index++;

// Echo
ash.Write(new AshFrame
{
    Control = new AshControl { Type = AshFrameType.Data, FrameNumber = index, AckNumber = index },
    Data = new byte[] { index, 0, 1, 0x81, 0x00, 3, 5, 6, 7 }
});
HandleResponse();
index++;

// JoinNetwork
ash.Write(new AshFrame
{
    Control = new AshControl { Type = AshFrameType.Data, FrameNumber = index, AckNumber = index },
    Data = new byte[]
    {
        index,
        0x00, 0x01,
        0x1F, 0x00,                                         // join network
        0x02,                                               // router
        //0x00, 0x12, 0x4B, 0x00, 0x29, 0xDD, 0xEC, 0xFB,     // extended PAN ID
        //0x1A, 0x62,                                         // PAN ID
        0xfb, 0xec, 0xdd, 0x29, 0x00, 0x4b, 0x12, 0x00,
        0x62, 0x1A,
        0x11,                                               // A power setting, in dBm  1 byte
        0x11,                                               // A radio channel.
        0x00,                                               // The method used to initially join the network.
        0x00,                                               // NWK Update ID.
        0x00, 0x00, 0x00, 0x00                              // NWK channel mask.        4 bytes
    }
});
HandleResponse();
index++;

// while (true)
// {
//     HandleResponse();
// }

Console.ReadLine();

void HandleResponse()
{
    var frame = ash.Read();
    if (frame.Control.Type == AshFrameType.Data)
    {
        var ackRequest = new AshFrame
        {
            Control = new AshControl { Type = AshFrameType.Ack, AckNumber = frame.Control.AckNumber }
        };
        ash.Write(ackRequest);
    }
}