namespace Bitjuice.EmberZNet.Ash;

public class AshFrame
{
    public AshControlByte Control { get; }
    public byte[] Data { get; }
    public AshFrameError? Error { get; }

    public bool IsValid => Error is null;

    public AshFrame(AshControlByte control, byte[] data, AshFrameError? error = null)
    {
        Control = control;
        Data = data;
        Error = error;
    }

    public static AshFrame Invalid(AshFrameError error)
    {
        AshControlByte.TryParse(0x00, out var control);
        return new AshFrame(control, Array.Empty<byte>(), error);
    }
}