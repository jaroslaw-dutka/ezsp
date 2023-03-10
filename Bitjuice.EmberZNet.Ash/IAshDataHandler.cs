namespace Bitjuice.EmberZNet.Ash;

public interface IAshDataHandler
{
    Task HandleAsync(ReadOnlyMemory<byte> data);
}