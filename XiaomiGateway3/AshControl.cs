using System.Text;

namespace XiaomiGateway3;

public class AshControl
{
    public AshFrameType Type { get; set; }
    public byte FrameNumber { get; set; }        // Data
    public bool Retransmission { get; set; }    // Data
    public byte AckNumber { get; set; }          // Data, Ack, Nak
    public bool Reserved { get; set; }          // Ack, Nak
    public bool NotReady { get; set; }          // Ack, Nak
    
    public static AshControl Parse(byte b)
    {
        var ctrl = new AshControl();
        if (b == 0xC2)
        {
            ctrl.Type = AshFrameType.Error;
        } 
        else if (b == 0xC1)
        {
            ctrl.Type = AshFrameType.Rstack;
        }
        else if (b == 0xC0)
        {
            ctrl.Type = AshFrameType.Rst;
        }
        else
        {
            ctrl.AckNumber = (byte)(b & 0x03);

            if ((b & 0x80) == 0)
            {
                ctrl.Type = AshFrameType.Data;
                ctrl.Retransmission = ((b >> 3) & 0x01) == 0x01;
                ctrl.FrameNumber = (byte)((b >> 4) & 0x03);
            }
            else
            {
                ctrl.Type = ((b >> 5) & 0x01) == 0x01
                    ? AshFrameType.Nak 
                    : AshFrameType.Ack;
                ctrl.NotReady = ((b >> 3) & 0x01) == 0x01;
                ctrl.Reserved = ((b >> 4) & 0x01) == 0x01;
            }
        }
        return ctrl;
    }

    public byte GetByte()
    {
        return 0;
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.Append(Type);
        if (Type is AshFrameType.Ack or AshFrameType.Nak)
        {
            sb.Append(NotReady ? ", NotReady " : ", Ready ");
            sb.Append($", AckNumber: {AckNumber}");
        }

        if (Type == AshFrameType.Data)
        {
            sb.Append($", FrameNumber: {FrameNumber} ");
            sb.Append($", AckNumber: {AckNumber} ");
            if (Retransmission)
                sb.Append(", Retransmission");
        }

        return sb.ToString();
    }
}