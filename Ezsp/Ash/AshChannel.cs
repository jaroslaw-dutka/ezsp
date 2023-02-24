using System.Buffers.Binary;
using Ezsp.Extensions;
using Ezsp.Utils;

namespace Ezsp.Ash;

public class AshChannel
{
    private const int BufferSize = 256;

    private byte escapeBit = 0x20;
    private readonly bool verbose;
    private readonly byte[] reservedBytes = Enum.GetValuesAsUnderlyingType<AshReservedByte>().OfType<byte>().ToArray();
    private readonly ManualResetEventSlim transmissionEnabled = new(true);
    private readonly byte[] readBuffer = new byte[BufferSize];
    private readonly byte[] writeBuffer = new byte[BufferSize];
    private readonly Stream stream;

    public AshChannel(Stream stream, bool verbose = false)
    {
        this.verbose = verbose;
        this.stream = new BufferedStream(stream, BufferSize);
    }

    public AshFrame? Read()
    {
        var length = ReadFrame(readBuffer);
        if (length < 0)
            return null;

        if (!AshControlByte.TryParse(readBuffer[0], out var ctrl))
            return null;

        var frame = new AshFrame
        {
            Control = ctrl,
            Data = new byte[length - 3]
        };

        readBuffer.AsSpan(1, length - 3).CopyTo(frame.Data);

        var computedCrc = Crc16.CcittFalse(readBuffer.AsSpan(0, length - 2));
        var receivedCrc = BinaryPrimitives.ReadUInt16BigEndian(readBuffer.AsSpan(length - 2, 2));

        if (computedCrc != receivedCrc)
            return null;

        switch (frame.Control.Type)
        {
            case AshFrameType.Data:
                if (frame.Data is null || !frame.Data.Length.IsBetween(3, 128))
                    return null;
                break;
            case AshFrameType.Ack:
            case AshFrameType.Nak:
            case AshFrameType.Rst:
                if (frame.Data is not null && frame.Data.Length > 0)
                    return null;
                break;
            case AshFrameType.Rstack:
            case AshFrameType.Error:
                if (frame.Data is null || frame.Data.Length != 2)
                    return null;
                break;
        }

        if (frame.Control.Type == AshFrameType.Data)
            AshPseudorandom.Xor(frame.Data);

        if (verbose)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("In " + DateTime.Now.ToString("O"));
            Console.WriteLine(frame);
        }

        return frame;
    }

    public void Write(AshFrame frame)
    {
        if (verbose)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Out " + DateTime.Now.ToString("O"));
            Console.WriteLine(frame);
        }

        var dataLength = frame.Data?.Length ?? 0;

        writeBuffer[0] = frame.Control.ToByte();
        if (frame.Data != null)
        {
            frame.Data.CopyTo(writeBuffer, 1);
            if (frame.Control.Type == AshFrameType.Data)
                AshPseudorandom.Xor(writeBuffer.AsSpan(1, frame.Data.Length));
        }

        var crc = Crc16.CcittFalse(writeBuffer.AsSpan(0, dataLength + 1));
        BinaryPrimitives.WriteUInt16BigEndian(writeBuffer.AsSpan(dataLength + 1, 2), crc);

        WriteFrame(writeBuffer.AsSpan(0, dataLength + 3));
    }

    public void WriteDiscard()
    {
        stream.WriteByte((byte)AshReservedByte.Cancel);
    }

    public void WriteReset()
    {
        Write(new AshFrame
        {
            Control = new AshControlByte
            {
                Type = AshFrameType.Rst
            }
        });
    }

    public void WriteData(byte frmNumber, byte ackNumber, byte[] data)
    {
        Write(new AshFrame
        {
            Control = new AshControlByte
            {
                Type = AshFrameType.Data,
                FrameNumber = frmNumber,
                AckNumber = ackNumber,
            },
            Data = data
        });
    }

    public void WriteAck(byte ackNumber)
    {
        Write(new AshFrame
        {
            Control = new AshControlByte
            {
                Type = AshFrameType.Ack,
                AckNumber = ackNumber,
            }
        });
    }

    public void WriteNak(byte ackNumber)
    {
        Write(new AshFrame
        {
            Control = new AshControlByte
            {
                Type = AshFrameType.Nak,
                AckNumber = ackNumber,
            }
        });
    }

    private int ReadFrame(Span<byte> buffer)
    {
        var index = 0;
        var endOfFrame = false;
        var substitute = false;
        var escape = false;
        while (!endOfFrame)
        {
            var b = (byte)stream.ReadByte();

            switch ((AshReservedByte)b)
            {
                case AshReservedByte.XON:
                    transmissionEnabled.Set();
                    break;
                case AshReservedByte.XOFF:
                    transmissionEnabled.Reset();
                    continue;
                case AshReservedByte.Substitute:
                    index = 0;
                    substitute = true;
                    continue;
                case AshReservedByte.Cancel:
                    index = 0;
                    continue;
                case AshReservedByte.Flag:
                    if (substitute)
                        substitute = false;
                    else
                        endOfFrame = true;
                    continue;
                case AshReservedByte.Escape:
                    escape = true;
                    continue;
            }

            if (escape)
            {
                b ^= escapeBit;
                escape = false;
            }

            if (substitute)
                continue;

            if (index < buffer.Length)
                buffer[index] = b;

            index++;
        }

        return index;
    }

    private void WriteFrame(Span<byte> buffer)
    {
        foreach (var b in buffer)
        {
            if (reservedBytes.Contains(b))
            {
                stream.WriteByte((byte)AshReservedByte.Escape);
                stream.WriteByte((byte)(b ^ escapeBit));
            }
            else
            {
                stream.WriteByte(b);
            }
        }
        stream.WriteByte(0x7E);

        if (!transmissionEnabled.IsSet)
            transmissionEnabled.Wait();

        stream.Flush();
    }
}