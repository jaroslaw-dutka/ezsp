using Bitjuice.EmberZNet.Client;
using System.Net.Sockets;

var cts = new CancellationTokenSource();

Console.CancelKeyPress += (sender, eventArgs) =>
{
    cts.Cancel();
    eventArgs.Cancel = true;
};

var tcp = new TcpClient();
await tcp.ConnectAsync("192.168.1.40", 8888, cts.Token);
var app = new TestApp(tcp.GetStream());
await app.RunAsync(cts.Token);