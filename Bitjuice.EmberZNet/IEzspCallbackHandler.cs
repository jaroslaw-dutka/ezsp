namespace Bitjuice.EmberZNet;

public interface IEzspCallbackHandler
{
    Task HandleCallbackAsync(ReadOnlyMemory<byte> data);
}