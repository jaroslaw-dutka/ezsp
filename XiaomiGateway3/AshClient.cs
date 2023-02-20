namespace XiaomiGateway3;

public class AshClient
{
    private readonly byte[] buffer = new byte[256];
    private readonly BinaryReader reader;
    private readonly BinaryWriter writer;

    public AshClient(Stream stream)
    {
        reader = new BinaryReader(stream);
        writer = new BinaryWriter(stream);
    }

    public AshFrame Read()
    {
        var length = ReadFrame(buffer);

        var frame = new AshFrame
        {
            Control = AshControl.Parse(buffer[0]),
            Data = new byte[length - 3],
            Crc = new byte[2]
        };

        buffer.AsSpan(1, length - 3).CopyTo(frame.Data);
        buffer.AsSpan(length - 2, 2).CopyTo(frame.Crc);

        return frame;
    }

    public void WriteData(byte[] data)
    {
        WriteFrame(0, data);
    }

    public void WriteAck(byte ackNum)
    {
        WriteFrame((byte) (0x80 | ackNum), Span<byte>.Empty);
    }

    public void WriteNack(byte ackNum)
    {
        WriteFrame((byte)(0xA0 | ackNum), Span<byte>.Empty);
    }

    public void WriteReset()
    {
        writer.Write((byte)AshSpecialByte.Discard);
        WriteFrame(0xC0, Span<byte>.Empty);
    }

    private int ReadFrame(Span<byte> data)
    {
        var index = 0;
        var substitute = false;
        var escape = false;
        while (true)
        {
            var b = reader.ReadByte();

            if (b == 0x11)
            {
                // XON: Resume transmission. Used in XON/XOFF flow control. Always ignored if received by the NCP.
                continue;
            }

            if (b == 0x13)
            {
                // XOFF: Stop transmission. Used in XON/XOFF flow control. Always ignored if received by the NCP.
                continue;
            }

            if (b == 0x18)
            {
                // Substitute Byte: Replaces a byte received with a low - level communication error(e.g., framing error) from the UART.When a Substitute Byte is processed, the data between the previous and the next Flag Bytes is ignored.
                index = 0;
                substitute = true;
                continue;
            }

            if (b == 0x1A)
            {
                // Cancel Byte: Terminates a frame in progress. A Cancel Byte causes all data received since the previous Flag Byte to be ignored. Note that as a special case, RST and RSTACK frames are preceded by Cancel Bytes to ignore any link startup noise.
                index = 0;
                continue;
            }

            if (b == 0x7E)
            {
                // Flag Byte: Marks the end of a frame. When a Flag Byte is received, the data received since the last Flag Byte or Cancel Byte is tested to see whether it is a valid frame.
                if (substitute)
                {
                    substitute = false;
                    continue;
                }
                break;
            }

            if (b == 0x7D)
            {
                // Escape Byte: Indicates that the following byte is escaped.If the byte after the Escape Byte is not a reserved byte, bit 5 of the byte is complemented to restore its original value. If the byte after the Escape Byte is a reserved value, the Escape Byte has no effect.
                escape = true;
                continue;
            }

            if (substitute)
            {
                continue;
            }

            if (escape)
            {
                b |= 0x20;
                escape = false;
            }

            data[index++] = b;
        }

        return index;
    }

    private void WriteFrame(byte ctrl, Span<byte> data)
    {
        // TODO: escaping
        var crc = Crc16.CcittFalse(new[] { ctrl });
        crc = Crc16.CcittFalse(crc, data);
        var crcBytes = BitConverter.GetBytes(crc);
        writer.Write(ctrl);
        writer.Write(data);
        writer.Write(new byte[] { crcBytes[1], crcBytes[0], 0x7E });
    }
}