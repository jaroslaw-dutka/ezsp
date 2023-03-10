namespace Bitjuice.EmberZNet.Ash;

public interface IAshWriter
{
    Task WriteDiscardAsync(CancellationToken cancellationToken);
    Task WriteAsync(byte ctrl, byte[] data, CancellationToken cancellationToken);
}