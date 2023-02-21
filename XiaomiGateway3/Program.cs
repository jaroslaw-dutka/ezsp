using System.Net.Sockets;
using XiaomiGateway3.Ash;

var tcp = new TcpClient();
tcp.Connect("192.168.1.40", 8888);
using var stream = tcp.GetStream();
var ash = new AshClient(stream);

ash.Reset();
ash.Write(new AshFrame
{
    Control = new AshControl { Type = AshFrameType.Rst }
});
HandleResponse();

byte index = 0;
ash.Write(new AshFrame
{
    Control = new AshControl { Type = AshFrameType.Data, FrameNumber = index, AckNumber = index },
    Data = new byte[] { 0, 0, 0, 4 }
});
HandleResponse();
index++;

for (var i = 0; i < 10; i++)
{
    ash.Write(new AshFrame
    {
        Control = new AshControl { Type = AshFrameType.Data, FrameNumber = index, AckNumber = index },
        Data = new byte[] { (byte)(i+1), 0, 1, 0, 0, 8 }
    });
    HandleResponse();
    index++;
}

// await Task.Delay(3000);

// ash.Write(new AshFrame
// {
//     Control = new AshControl { Type = AshFrameType.Data, FrameNumber = index, AckNumber = index },
//     Data = new byte[]
//     {
//         0x01,
//         0x00, 0x00,
//         0x00, 0x01F,                                        // join network
//         0x02,                                               // router
//         0x00, 0x12, 0x4B, 0x00, 0x29, 0xDD, 0xEC, 0xFB,     // extended PAN ID
//         0x1A, 0x62,                                         // PAN ID
//         0x11,                                               // A power setting, in dBm  1 byte
//         0x11,                                               // A radio channel.
//         0x00,                                               // The method used to initially join the network.
//         0x00,                                               // NWK Update ID.
//         0x00, 0x00, 0x00, 0x00                              // NWK channel mask.        4 bytes
//     }
// });
// HandleResponse();
// index++;

while (true)
{
    HandleResponse();
}

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

    Console.WriteLine(frame.ToString());
}