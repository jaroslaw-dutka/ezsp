namespace XiaomiGateway3;

public class AshReader
{
    private byte[] random = {
        0x42, 0x21, 0xA8, 0x54, 0x2A, 0x15, 0xB2, 0x59, 0x94, 0x4A, 0x25, 0xAA, 0x55, 0x92, 0x49, 0x9C, 0x4E, 0x27, 0xAB, 0xED, 0xCE, 0x67, 0x8B, 0xFD, 0xC6,
        0x63, 0x89, 0xFC, 0x7E, 0x3F, 0xA7, 0xEB, 0xCD, 0xDE, 0x6F, 0x8F, 0xFF, 0xC7, 0xDB, 0xD5, 0xD2, 0x69, 0x8C, 0x46, 0x23, 0xA9, 0xEC, 0x76, 0x3B, 0xA5,
        0xEA, 0x75, 0x82, 0x41, 0x98, 0x4C, 0x26, 0x13, 0xB1, 0xE0, 0x70, 0x38, 0x1C, 0x0E, 0x07, 0xBB, 0xE5, 0xCA, 0x65, 0x8A, 0x45, 0x9A, 0x4D, 0x9E, 0x4F,
        0x9F, 0xF7, 0xC3, 0xD9, 0xD4, 0x6A, 0x35, 0xA2, 0x51, 0x90, 0x48, 0x24, 0x12, 0x09, 0xBC, 0x5E, 0x2F, 0xAF, 0xEF, 0xCF, 0xDF, 0xD7, 0xD3, 0xD1, 0xD0
    };
    
    private readonly BinaryReader reader;

    public AshReader(BinaryReader reader)
    {
        this.reader = reader;
    }

    public AshFrame Read()
    {
        var buffer = new byte[1024];
        var length = ReadPacket(buffer);

        var frame = new AshFrame
        {
            Control = new AshControl(buffer[0]),
            Data = new byte[length-3],
            Crc = new byte[2]
        };

        buffer.AsSpan(1, length-3).CopyTo(frame.Data);
        buffer.AsSpan(length - 2, 2).CopyTo(frame.Crc);

        if (frame.Control.Type == AshFrameType.Data)
        {
            for (var i = 0; i < frame.Data.Length; i++)
            {
                frame.Data[i] = (byte)(frame.Data[i] ^ random[i]);
            }
        }

        return frame;
    }

    private int ReadPacket(byte[] buffer)
    {
        var index = 0;
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
                // TODO:
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
                break;
            }

            if (b == 0x7D)
            {
                // Escape Byte: Indicates that the following byte is escaped.If the byte after the Escape Byte is not a reserved byte, bit 5 of the byte is complemented to restore its original value. If the byte after the Escape Byte is a reserved value, the Escape Byte has no effect.
                escape = true;
                continue;
            }

            if (escape)
            {
                b |= 0x20;
                escape = false;
            }

            buffer[index++] = b;
        }

        return index;
    }
}