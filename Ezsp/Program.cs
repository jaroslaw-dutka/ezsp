using System.Net.Sockets;
using Ezsp;
using Ezsp.Ash;

var tcp = new TcpClient();
tcp.Connect("192.168.1.40", 8888);
var stream = tcp.GetStream();
var ash = new AshClient(stream, true);
var controller = new AshController(ash);
var client = new EzspClientV2(controller);

await client.ConnectAsync();

// await client.SendAsync(EzspCommand.Version, 7);

// Echo
// await client.SendAsync(EzspCommand.Echo, 3, 1, 2, 3);

// Bootloader
// await client.SendSync(new byte[] { index++, 0, 1, 0x8f, 0, 0 });

// setTimer
// await client.SendSync(new byte[] { index++, 0, 1, 0x0e, 0, 0, 100, 0, 1, 1 });

// NOP
//await client.SendAsync(EzspCommand.Nop);

// Delay
//await client.SendSync(new byte[] { index++, 0, 1, 0x9D, 0, 0, 10 });

for (var i = 0; i < 5; i++)
{
    client.SendAsync(EzspCommand.Nop);
    //await Task.Delay(3000);
}

await Task.Delay(200000);

// ezsp.Send(EzspCommand.Version, 8);
// ezsp.Read();
//
// ezsp.Send(EzspCommand.Echo, 3, 7, 8, 9);
// ezsp.Read();
//
// ezsp.Send(EzspCommand.JoinNetwork, new byte[]
// {
//     0x02, // router
//     //0x00, 0x12, 0x4B, 0x00, 0x29, 0xDD, 0xEC, 0xFB,     // extended PAN ID
//     //0x1A, 0x62,                                         // PAN ID
//     0xfb, 0xec, 0xdd, 0x29, 0x00, 0x4b, 0x12, 0x00,
//     0x62, 0x1A,
//     0x11, // A power setting, in dBm  1 byte
//     0x11, // A radio channel.
//     0x00, // The method used to initially join the network.
//     0x00, // NWK Update ID.
//     0x00, 0x00, 0x00, 0x00 // NWK channel mask.        4 bytes
// });
// ezsp.Read();

Console.ReadLine();