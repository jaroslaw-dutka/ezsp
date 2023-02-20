using System.Net.Sockets;

var client = new TcpClient();
client.Connect("192.168.1.40", 8888);
var stream = client.GetStream();
var reader = new BinaryReader(stream);
var writer = new BinaryWriter(stream);

// var r = 0x42;
// for (var i = 0; i < 100; i++)
// {
//     Console.Write($"0x{r:X2}, ");
//     if ((r & 1) == 1)
//     {
//         r = (r >> 1) ^ 0xB8;
//     }
//     else
//     {
//         r = r >> 1;
//     }
// }

// writer.Write(new byte[]
// {
//     0x1A,0xC0,0x38,0xBC,0x7E
// });

var ashReader = new AshReader(reader);

while (true)
{
    var frame = ashReader.Read();
    Console.WriteLine(frame.ToString());
}
