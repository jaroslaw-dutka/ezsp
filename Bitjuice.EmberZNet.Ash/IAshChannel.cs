namespace Bitjuice.EmberZNet.Ash;

public interface IAshChannel
{
    Task ConnectAsync(IAshDataHandler handler, CancellationToken cancellationToken);
    Task DisconnectAsync();
    void Send(byte[] data);
}