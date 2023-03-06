using System.Buffers.Binary;
using Bitjuice.EmberZNet.Ash.Utils;

namespace Bitjuice.EmberZNet.Ash;

public class AshReader
{
    private const byte EscapeBit = 0x20;

    private readonly Stream stream;
    private readonly bool verbose;
    private readonly byte[] buffer;

    public AshReader(Stream stream, int bufferSize = 256, bool verbose = false)
    {
        this.stream = stream;
        this.verbose = verbose;
        buffer = new byte[bufferSize];
    }

    public async Task<AshFrame> ReadAsync(CancellationToken cancellationToken)
    {
        var length = await ReadFrameAsync(cancellationToken);

        if (length < 0)
            return AshFrame.Invalid(AshFrameError.EndOfStream);

        if (length < 3)
            return AshFrame.Invalid(AshFrameError.MessageTooShort);

        if (length > buffer.Length)
            return AshFrame.Invalid(AshFrameError.BufferOverflow);

        if (!AshControlByte.TryParse(buffer[0], out var ctrl))
            return AshFrame.Invalid(AshFrameError.InvalidControl);

        var data = new byte[length - 3];

        buffer.AsSpan(1, length - 3).CopyTo(data);

        var computedCrc = Crc16.CcittFalse(buffer.AsSpan(0, length - 2));
        var receivedCrc = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan(length - 2, 2));
        
        switch (ctrl.Type)
        {
            case AshFrameType.Data:
                if (!data.Length.IsBetween(3, 128))
                    return AshFrame.Invalid(AshFrameError.InvalidPayloadSize);
                break;
            case AshFrameType.Ack:
            case AshFrameType.Nak:
            case AshFrameType.Reset:
                if (data.Length > 0)
                    return AshFrame.Invalid(AshFrameError.InvalidPayloadSize);
                break;
            case AshFrameType.ResetAck:
            case AshFrameType.Error:
                if (data.Length != 2)
                    return AshFrame.Invalid(AshFrameError.InvalidPayloadSize);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (ctrl.Type == AshFrameType.Data)
            AshRandom.Xor(data);

        if (computedCrc != receivedCrc)
        {
            if (verbose)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("In " + DateTime.Now.ToString("O"));
                Console.WriteLine($"Ctrl: {ctrl}");
                Console.WriteLine($"Data: {BitConverter.ToString(data).Replace("-", " ")}");
                // Console.WriteLine($"Buffer: {BitConverter.ToString(buffer.AsSpan(0, length).ToArray()).Replace("-", " ")}");
                // Console.WriteLine($"Crc: {receivedCrc.ToString("X").Replace("-", " ")}");
                Console.WriteLine();
            }

            return AshFrame.Invalid(AshFrameError.InvalidCrc);
        }

        if (verbose)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("In " + DateTime.Now.ToString("O"));
            Console.WriteLine($"Ctrl: {ctrl}");
            Console.WriteLine($"Data: {BitConverter.ToString(data).Replace("-", " ")}");
            // Console.WriteLine($"Buffer: {BitConverter.ToString(buffer.AsSpan(0, length).ToArray()).Replace("-", " ")}");
            // Console.WriteLine($"Crc: {receivedCrc.ToString("X").Replace("-", " ")}");
            Console.WriteLine();
        }

        return new AshFrame(ctrl, data);
    }

    private async Task<int> ReadFrameAsync(CancellationToken cancellationToken)
    {
        var index = 0;
        var endOfFrame = false;
        var substitute = false;
        var escape = false;
        while (!endOfFrame)
        {
            var b = await ReadByteAsync(cancellationToken);
            if (b < 0)
                return -1;

            switch ((AshReservedByte)b)
            {
                case AshReservedByte.XON:
                case AshReservedByte.XOFF:
                    // TODO: handle software transmission control
                    continue;
                case AshReservedByte.Substitute:
                    index = 0;
                    substitute = true;
                    continue;
                case AshReservedByte.Cancel:
                    index = 0;
                    continue;
                case AshReservedByte.Flag:
                    if (index == 0)
                        continue;
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
                b ^= EscapeBit;
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

    public async Task<int> ReadByteAsync(CancellationToken cancellationToken)
    {
        var array = new byte[1];
        var bytes = await stream.ReadAsync(array, 0, 1, cancellationToken);
        if (bytes == 0)
            return -1;
        return array[0];
    }
}