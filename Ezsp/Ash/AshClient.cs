using System.Buffers.Binary;

namespace Ezsp.Ash;

public class AshClient
{
    private const int BufferSize = 256;

    private readonly bool verbose;
    private readonly byte[] specialBytes = Enum.GetValuesAsUnderlyingType<AshCtrlByte>().OfType<byte>().ToArray();
    private readonly byte[] readBuffer = new byte[BufferSize];
    private readonly byte[] writeBuffer = new byte[BufferSize];
    private readonly BinaryReader reader;
    private readonly BinaryWriter writer;

    public AshClient(Stream stream, bool verbose = false)
    {
        this.verbose = verbose;
        var bufferedStream = new BufferedStream(stream, BufferSize);
        reader = new BinaryReader(bufferedStream);
        writer = new BinaryWriter(bufferedStream);
    }

    public void Reset()
    {
        writer.Write((byte)AshCtrlByte.Discard);
    }

    public AshFrame Read()
    {
        var length = ReadFrame(readBuffer);

        var frame = new AshFrame
        {
            Control = AshControl.Parse(readBuffer[0]),
            Data = new byte[length - 3]
        };

        readBuffer.AsSpan(1, length - 3).CopyTo(frame.Data);

        var computedCrc = Crc16.CcittFalse(readBuffer.AsSpan(0, length - 2));
        var receivedCrc = BinaryPrimitives.ReadUInt16BigEndian(readBuffer.AsSpan(length - 2, 2));

        if (computedCrc != receivedCrc)
            throw new Exception("Invalid CRC");

        if (frame.Control.Type == AshFrameType.Data) 
            AshRandom.Replace(frame.Data);

        if (verbose)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("In");
            Console.WriteLine(frame);
        }

        return frame;
    }

    public void Write(AshFrame frame)
    {
        if (verbose)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Out");
            Console.WriteLine(frame);
        }

        var dataLength = frame.Data?.Length ?? 0;

        writeBuffer[0] = frame.Control.ToByte();
        if (frame.Data != null)
        {
            frame.Data.CopyTo(writeBuffer, 1);
            if (frame.Control.Type == AshFrameType.Data)
                AshRandom.Replace(writeBuffer.AsSpan(1, frame.Data.Length));
        }
        var crc = Crc16.CcittFalse(writeBuffer.AsSpan(0, dataLength + 1));
        BinaryPrimitives.WriteUInt16BigEndian(writeBuffer.AsSpan(dataLength + 1, 2), crc);
        
        WriteFrame(writeBuffer.AsSpan(0, dataLength + 3));
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

            switch ((AshCtrlByte)b)
            {
                case AshCtrlByte.Resume:
                case AshCtrlByte.Stop:
                    // TODO
                    continue;
                case AshCtrlByte.Substitute:
                    index = 0;
                    substitute = true;
                    continue;
                case AshCtrlByte.Discard:
                    index = 0;
                    continue;
                case AshCtrlByte.EndOfFrame:
                    if (substitute)
                        substitute = false;
                    else
                        endOfFrame = true;
                    continue;
                case AshCtrlByte.Escape:
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
                writer.Write((byte)AshCtrlByte.Escape);
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