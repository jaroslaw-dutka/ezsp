namespace XiaomiGateway3.Ash;

public class AshClient
{
    private readonly byte[] specialBytes = Enum.GetValuesAsUnderlyingType<AshSpecialByte>().OfType<byte>().ToArray();
    private readonly byte[] readBuffer = new byte[256];
    private readonly byte[] writeBuffer = new byte[256];
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
        Array.Fill<byte>(readBuffer, 0);
        var length = ReadFrame(readBuffer);

        var frame = new AshFrame
        {
            Control = AshControl.Parse(readBuffer[0]),
            Data = new byte[length - 3]
        };

        readBuffer.AsSpan(1, length - 3).CopyTo(frame.Data);

        var crc = Crc16.CcittFalse(readBuffer.AsSpan(0, length - 2));
        var crcBytes = BitConverter.GetBytes(crc);
        if (crcBytes[0] != readBuffer[length - 1] || crcBytes[1] != readBuffer[length - 2])
            throw new Exception("Invalid CRC");

        if (frame.Control.Type == AshFrameType.Data)
        {
            AshRandom.Replace(frame.Data);
        }

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine("In");
        Console.WriteLine(frame);

        return frame;
    }

    public void Write(AshFrame frame)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("Out");
        Console.WriteLine(frame);

        Array.Fill<byte>(writeBuffer, 0);
        var dataLength = frame.Data?.Length ?? 0;

        writeBuffer[0] = frame.Control.ToByte();
        if (frame.Data != null)
        {
            frame.Data.CopyTo(writeBuffer, 1);
            if (frame.Control.Type == AshFrameType.Data)
                AshRandom.Replace(writeBuffer.AsSpan(1, frame.Data.Length));
        }
        var crc = Crc16.CcittFalse(writeBuffer.AsSpan(0, dataLength + 1));
        var crcBytes = BitConverter.GetBytes(crc);
        writeBuffer[dataLength + 1] = crcBytes[1];
        writeBuffer[dataLength + 2] = crcBytes[0];
        
        WriteFrame(writeBuffer.AsSpan(0, dataLength + 3));
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
                b ^= 0x20;
                escape = false;
            }

            data[index++] = b;
        }

        return index;
    }

    private void WriteFrame(Span<byte> data)
    {
        foreach (var b in data)
        {
            if (specialBytes.Contains(b))
            {
                writer.Write((byte)AshSpecialByte.Escape);
                writer.Write((byte)(b ^ 0x20));
            }
            else
            {
                writer.Write(b);
            }
        }
        writer.Write((byte)0x7E);
        writer.Flush();
    }
}