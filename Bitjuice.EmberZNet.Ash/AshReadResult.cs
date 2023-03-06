namespace Bitjuice.EmberZNet.Ash;

public class AshReadResult
{
    public AshReadError? Error { get; private init; }
    public AshFrame? Frame { get; private init; }

    private AshReadResult()
    {
    }

    public static AshReadResult Success(AshFrame frame) => new() { Frame = frame };
    public static AshReadResult Fail(AshReadError error) => new() { Error = error };
}