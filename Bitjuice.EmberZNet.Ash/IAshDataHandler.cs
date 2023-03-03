namespace Bitjuice.EmberZNet.Ash;

public interface IAshDataHandler
{
    Task HandleAsync(byte[] data);
}