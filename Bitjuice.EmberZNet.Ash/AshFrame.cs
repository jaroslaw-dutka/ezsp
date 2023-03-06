namespace Bitjuice.EmberZNet.Ash;

public class AshFrame
{
    public AshCtrl Ctrl { get; }
    public byte[] Data { get; }

    public AshFrame(AshCtrl ctrl, byte[] data)
    {
        Ctrl = ctrl;
        Data = data;
    }
}