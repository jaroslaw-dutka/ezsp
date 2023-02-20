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

ash.Write(new AshFrame
{
    Control = new AshControl { Type = AshFrameType.Data },
    Data = new byte[] { 0, 0, 0, 4 }
});

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