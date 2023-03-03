using Bitjuice.EmberZNet.Client;

var app = new TestApp();
await app.ConnectAsync(CancellationToken.None);
await app.RunAsync(CancellationToken.None);
await Task.Delay(1000000);