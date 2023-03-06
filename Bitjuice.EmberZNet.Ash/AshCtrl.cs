using System.Text;

namespace Bitjuice.EmberZNet.Ash;

public class AshCtrl
{
    private const byte AckMask = 0xE0;
    private const byte DataMask = 0x80;

    public AshFrameType Type { get; set; }
    public byte FrameNumber { get; set; }        // Data
    public byte AckNumber { get; set; }          // Data, Ack, Nak
    public bool Retransmission { get; set; }     // Data
    public bool NotReady { get; set; }           // Ack, Nak

    private AshCtrl()
    {
    }

    public static byte Reset() => (byte)AshFrameType.Reset;
    public static byte Data(byte frmNum, byte ackNum, bool retry) => (byte)((frmNum & 0x07) << 4 | (retry ? 0x08 : 0x00) | (ackNum & 0x07));
    public static byte Ack(byte ackNum, bool notReady) => (byte)((byte)AshFrameType.Ack | (notReady ? 0x08 : 0x00) | (ackNum & 0x07));
    public static byte Nak(byte ackNum, bool notReady) => (byte)((byte)AshFrameType.Nak | (notReady ? 0x08 : 0x00) | (ackNum & 0x07));

    public static bool TryParse(byte b, out AshCtrl ctrl)
    {
        ctrl = new AshCtrl();
        switch (b)
        {
            case (byte)AshFrameType.Error:
            case (byte)AshFrameType.ResetAck:
            case (byte)AshFrameType.Reset:
                ctrl.Type = (AshFrameType)b;
                break;
            default:
            {
                if ((b & AckMask) == (byte)AshFrameType.Ack)
                {
                    ctrl.Type = AshFrameType.Ack;
                    ctrl.AckNumber = (byte)(b & 0x07);
                    ctrl.NotReady = ((b >> 3) & 0x01) == 0x01;
                }
                else if ((b & AckMask) == (byte)AshFrameType.Nak)
                {
                    ctrl.Type = AshFrameType.Nak;
                    ctrl.AckNumber = (byte)(b & 0x07);
                    ctrl.NotReady = ((b >> 3) & 0x01) == 0x01;
                }
                else if ((b & DataMask) == (byte)AshFrameType.Data)
                {
                    ctrl.Type = AshFrameType.Data;
                    ctrl.AckNumber = (byte)(b & 0x07);
                    ctrl.Retransmission = ((b >> 3) & 0x01) == 0x01;
                    ctrl.FrameNumber = (byte)((b >> 4) & 0x07);
                }
                else
                    return false;

                break;
            }
        }

        return true;
    }

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