using System.Net.Sockets;
using Ezsp;

var tcp = new TcpClient();
tcp.Connect("192.168.1.40", 8888);
var client = new EzspClient(tcp.GetStream());

await client.ConnectAsync(CancellationToken.None);

// await client.SendAsync(EzspCommand.Version, 7);

// Echo
 // await client.SendAsync(EzspCommand.Echo, 3, 1, 2, 3);

// Bootloader
// await client.SendSync(new byte[] { index++, 0, 1, 0x8f, 0, 0 });

// setTimer
await client.SendAsync(EzspCommand.SetTimer, 0, 100, 0, 1, 1);

// NOP
// await client.SendAsync(EzspCommand.Nop);

// Delay
//await client.SendAsync(new byte[] { index++, 0, 1, 0x9D, 0, 0, 10 });

for (var i = 0; i < 200; i++)
{
    // await client.SendAsync(EzspCommand.Nop);
    var aa = await client.SendAsync(EzspCommand.Version, 7);
    if (aa[5] != 0x07)
        throw new Exception("aaa");
    // await Task.Delay(3000);
}

// await Task.Delay(3000);
// await client.ConnectAsync(CancellationToken.None);
// await client.SendAsync(EzspCommand.Echo, 3, 1, 2, 3);

//await client.SendAsync(EzspCommand.JoinNetwork, new byte[]
//{
//    0x02, // router
//    //0x00, 0x12, 0x4B, 0x00, 0x29, 0xDD, 0xEC, 0xFB,     // extended PAN ID
//    //0x1A, 0x62,                                         // PAN ID
//    0xfb, 0xec, 0xdd, 0x29, 0x00, 0x4b, 0x12, 0x00,
//    0x62, 0x1A,
//    0x11, // A power setting, in dBm  1 byte
//    0x11, // A radio channel.
//    0x00, // The method used to initially join the network.
//    0x00, // NWK Update ID.
//    0x00, 0x00, 0x00, 0x00 // NWK channel mask.        4 bytes
//});

await Task.Delay(200000);

Console.ReadLine();