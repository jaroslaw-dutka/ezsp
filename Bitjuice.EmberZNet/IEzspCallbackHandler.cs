namespace Bitjuice.EmberZNet;

public interface IEzspCallbackHandler
{
    Task HandleCallbackAsync(byte[] data);
}