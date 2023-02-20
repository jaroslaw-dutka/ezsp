using System.Net.Sockets;
using XiaomiGateway3;

var tcp = new TcpClient();
tcp.Connect("192.168.1.40", 8888);
using var stream = tcp.GetStream();
var ash = new AshClient(stream);

ash.WriteReset();

{
    var frame = ash.Read();
    ash.WriteAck(frame.Control.AckNumber);
    Console.WriteLine(frame.ToString());
}

var data = new byte[] { 0, 0, 0, 4 };
PseudoRandom.ReplaceInplace(data);
ash.WriteData(data);

while (true)
{
    var frame = ash.Read();
    if (frame.Control.Type == AshFrameType.Data)
    {
        ash.WriteAck(frame.Control.AckNumber);
    }

    PseudoRandom.ReplaceInplace(frame.Data);

    Console.WriteLine(frame.ToString());
}