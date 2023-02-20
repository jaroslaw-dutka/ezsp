namespace XiaomiGateway3.Ash;

public class AshClient
{
    private readonly byte[] buffer = new byte[256];
    private readonly BinaryReader reader;
    private readonly BinaryWriter writer;

    public AshClient(Stream stream)
    {
        var bufferedStream = new BufferedStream(stream, 256);
        reader = new BinaryReader(bufferedStream);
        writer = new BinaryWriter(bufferedStream);
    }

    public AshFrame Read()
    {
        var length = ReadFrame(buffer);

        var frame = new AshFrame
        {
            Control = AshControl.Parse(buffer[0]),
            Data = new byte[length - 3]
        };

        buffer.AsSpan(1, length - 3).CopyTo(frame.Data);

        var crc = Crc16.CcittFalse(buffer.AsSpan(0, length - 2));
        var crcBytes = BitConverter.GetBytes(crc);
        if (crcBytes[0] != buffer[length - 1] || crcBytes[1] != buffer[length - 2])
            throw new Exception("Invalid CRC");

        if (frame.Control.Type == AshFrameType.Data)
        {
            AshRandom.ReplaceInplace(frame.Data);
        }

        return frame;
    }

    public void Write(AshFrame frame)
    {
        var ctrl = frame.Control.GetByte();
        var data = frame.Control.Type == AshFrameType.Data 
            ? AshRandom.Replace(frame.Data) 
            : frame.Data;
        var crc = Crc16.CcittFalse(new[] { ctrl });
        crc = Crc16.CcittFalse(crc, data);
        var crcBytes = BitConverter.GetBytes(crc);
        crcBytes = crcBytes.Reverse().ToArray();
        WriteFrame(ctrl, data, crcBytes);
    }

    public void Reset()
    {
        writer.Write((byte)AshSpecialByte.Discard);
    }

    private int ReadFrame(Span<byte> data)
    {
        var index = 0;
        var endOfFrame = false;
        var substitute = false;
        var escape = false;
        while (!endOfFrame)
        {
            var b = reader.ReadByte();

            switch ((AshSpecialByte)b)
            {
                case AshSpecialByte.Resume:
                case AshSpecialByte.Stop:
                    continue;
                case AshSpecialByte.Substitute:
                    index = 0;
                    substitute = true;
                    continue;
                case AshSpecialByte.Discard:
                    index = 0;
                    continue;
                case AshSpecialByte.EndOfFrame:
                    if (substitute)
                        substitute = false;
                    else
                        endOfFrame = true;
                    continue;
                case AshSpecialByte.Escape:
                    escape = true;
                    continue;
            }

            if (substitute)
                continue;

            if (escape)
            {
                b |= 0x20;
                escape = false;
            }

            data[index++] = b;
        }

        return index;
    }

    private void WriteFrame(byte ctrl, Span<byte> data, Span<byte> crc)
    {
        // TODO: escaping
        writer.Write(ctrl);
        writer.Write(data);
        writer.Write(crc);
        writer.Write((byte)0x7E);
        writer.Flush();
    }
}