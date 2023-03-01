using Bitjuice.EmberZNet.Client;

var app = new TestApp();
await app.ConnectAsync();
await app.RunAsync();
await Task.Delay(1000000);