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

public class AshFrameFactory
{
    public static AshFrameBase Create(byte ctrl, byte[] data)
    {
        return null;
    }
}

public abstract class AshFrameBase
{
    public abstract byte Control { get; }

    public AshFrameType Type { get; }
    public byte[] Data { get; }

    public AshFrameError? Error { get; }
    public bool IsValid => Error is null;

    protected AshFrameBase(AshFrameType type, byte[] data)
    {
        Type = type;
        Data = data;
    }
}

public class AshFrameAck : AshFrameBase
{
    public override byte Control => 0x80;

    public int AckNum { get; }

    public AshFrameAck(int ackNum) : base(AshFrameType.Ack, Array.Empty<byte>())
    {
        AckNum = ackNum;
    }

    
}

public class AshFrameNak : AshFrameBase
{
    public override byte Control => 0xA0;

    public int AckNum { get; }

    public AshFrameNak(int ackNum) : base(AshFrameType.Nak, Array.Empty<byte>())
    {
        AckNum = ackNum;
    }
}

public class AshFrameData : AshFrameBase
{
    public override byte Control => 0x00;

    public int FrmNum { get; }
    public int AckNum { get; }
    public bool Retry { get; set; }

    public AshFrameData(int frmNum, int ackNum, bool retry, byte[] data) : base(AshFrameType.Data, data)
    {
        FrmNum = frmNum;
        AckNum = ackNum;
        Retry = retry;
    }
}

public class AshFrameReset : AshFrameBase
{
    public override byte Control => 0xC0;

    public AshFrameReset() : base(AshFrameType.Reset, Array.Empty<byte>())
    {
    }
}

public class AshFrameResetAck : AshFrameBase
{
    public override byte Control => 0xC1;

    public AshFrameResetAck() : base(AshFrameType.ResetAck, Array.Empty<byte>())
    {
    }
}

public class AshFrameError1 : AshFrameBase
{
    public override byte Control => 0x00;

    public AshFrameError1() : base(AshFrameType.ResetAck, Array.Empty<byte>())
    {
    }
}