namespace Bitjuice.EmberZNet.Ash;

public interface IAshReader
{
    Task<AshReadResult> ReadAsync(CancellationToken cancellationToken);
}