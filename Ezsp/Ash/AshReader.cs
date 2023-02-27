using System.Buffers.Binary;
using EzspLib.Utils;

namespace EzspLib.Ash;

public class AshReader
{
    private readonly byte escapeBit = 0x20;
    private readonly bool verbose;
    private readonly ManualResetEventSlim transmissionEnabled = new(true);
    private readonly byte[] readBuffer;
    private readonly Stream stream;

    public AshReader(Stream stream, int bufferSize = 256, bool verbose = false)
    {
        this.stream = stream;
        this.verbose = verbose;
        readBuffer = new byte[bufferSize];
    }

    public async Task<AshFrame?> ReadAsync(CancellationToken cancellationToken)
    {
        var length = await ReadFrameAsync(readBuffer, cancellationToken);
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

    private async Task<int> ReadFrameAsync(byte[] buffer, CancellationToken cancellationToken)
    {
        var index = 0;
        var endOfFrame = false;
        var substitute = false;
        var escape = false;
        while (!endOfFrame)
        {
            var b = await stream.ReadByteAsync(cancellationToken);
            if (b < 0)
                return -1;

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
                buffer[index] = (byte)b;

            index++;
        }

        return index;
    }
}