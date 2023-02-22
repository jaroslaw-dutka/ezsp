using System.Text;

namespace Ezsp.Ash;

public class AshControl
{
    public AshFrameType Type { get; set; }
    public byte FrameNumber { get; set; }        // Data
    public bool Retransmission { get; set; }     // Data
    public byte AckNumber { get; set; }          // Data, Ack, Nak
    public bool Reserved { get; set; }           // Ack, Nak
    public bool NotReady { get; set; }           // Ack, Nak

    public static AshControl Parse(byte b)
    {
        var ctrl = new AshControl();
        switch (b)
        {
            case (byte)AshFrameType.Error:
            case (byte)AshFrameType.Rstack:
            case (byte)AshFrameType.Rst:
                ctrl.Type = (AshFrameType)b;
                break;
            default:
            {
                if ((b & (byte)AshFrameTypeMask.Ack) == (byte)AshFrameType.Ack)
                {
                    ctrl.Type = AshFrameType.Ack;
                    ctrl.AckNumber = (byte)(b & 0x07);
                    ctrl.NotReady = ((b >> 3) & 0x01) == 0x01;
                    ctrl.Reserved = ((b >> 4) & 0x01) == 0x01;
                }
                else if ((b & (byte)AshFrameTypeMask.Nak) == (byte)AshFrameType.Nak)
                {
                    ctrl.Type = AshFrameType.Nak;
                    ctrl.AckNumber = (byte)(b & 0x07);
                    ctrl.NotReady = ((b >> 3) & 0x01) == 0x01;
                    ctrl.Reserved = ((b >> 4) & 0x01) == 0x01;
                }
                else if ((b & (byte)AshFrameTypeMask.Data) == (byte)AshFrameType.Data)
                {
                    ctrl.Type = AshFrameType.Data;
                    ctrl.AckNumber = (byte)(b & 0x07);
                    ctrl.Retransmission = ((b >> 3) & 0x01) == 0x01;
                    ctrl.FrameNumber = (byte)((b >> 4) & 0x07);
                }
                else
                    throw new ArgumentOutOfRangeException(nameof(b));

                break;
            }
        }

        return ctrl;
    }

    public byte ToByte() => Type switch
    {
        AshFrameType.Data => (byte)((byte)Type | (FrameNumber & 0x07) << 4 | (Retransmission ? 0x08 : 0x00) | (AckNumber & 0x07)),
        AshFrameType.Ack => (byte)((byte)Type | (Reserved ? 0x10 : 0x00) | (NotReady ? 0x08 : 0x00) | (AckNumber & 0x07)),
        AshFrameType.Nak => (byte)((byte)Type | (Reserved ? 0x10 : 0x00) | (NotReady ? 0x08 : 0x00) | (AckNumber & 0x07)),
        AshFrameType.Rst => (byte)Type,
        AshFrameType.Rstack => (byte)Type,
        AshFrameType.Error => (byte)Type,
        _ => throw new ArgumentOutOfRangeException()
    };

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Type);
        if (Type is AshFrameType.Ack or AshFrameType.Nak)
        {
            sb.Append(NotReady ? ", NotReady" : ", Ready");
            sb.Append($", AckNumber: {AckNumber}");
        }

        if (Type == AshFrameType.Data)
        {
            sb.Append($", FrameNumber: {FrameNumber}");
            sb.Append($", AckNumber: {AckNumber}");
            if (Retransmission)
                sb.Append(", Retransmission");
        }

        return sb.ToString();
    }
}