using System.Text;

public class AshControl
{
    public AshFrameType Type { get; set; }
    public int AckNumber { get; set; }
    public int FrameNumber { get; set; }
    // public bool Reserved { get; set; }
    public bool NotReady { get; set; }
    public bool Retransmission { get; set; }
    

    public AshControl(byte b)
    {
        if (b == 0xC2)
        {
            Type = AshFrameType.Error;
        } 
        else if (b == 0xC1)
        {
            Type = AshFrameType.Rstack;
        }
        else if (b == 0xC0)
        {
            Type = AshFrameType.Rst;
        }
        else
        {
            AckNumber = b & 0x03;

            if ((b & 0x80) == 0)
            {
                Type = AshFrameType.Data;
                Retransmission = ((b >> 3) & 0x01) == 0x01;
                FrameNumber = (b >> 4) & 0x03;
            }
            else
            {
                Type = ((b >> 5) & 0x01) == 0x01
                    ? AshFrameType.Nak 
                    : AshFrameType.Ack;
                NotReady = ((b >> 3) & 0x01) == 0x01;
                // Reserved = ((b >> 4) & 0x01) == 0x01;
            }
        }
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