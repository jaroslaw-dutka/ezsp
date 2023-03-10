using System.Buffers.Binary;
using Bitjuice.EmberZNet.Ash.Utils;

namespace Bitjuice.EmberZNet.Ash;

public class AshReader : IAshReader
{
    private readonly Stream stream;
    
    private readonly byte[] buffer;

    public bool Verbose { get; set; }

    public AshReader(Stream stream, int bufferSize = AshConstants.MaxMessageSize + 1)
    {
        this.stream = stream;
        buffer = new byte[bufferSize];
    }

    public async Task<AshReadResult> ReadAsync(CancellationToken cancellationToken)
    {
        var length = await ReadFrameAsync(cancellationToken);

        if (length < 0)
            return AshReadResult.Fail(AshReadError.EndOfStream);

        if (length < 3)
            return AshReadResult.Fail(AshReadError.MessageTooShort);

        if (length > buffer.Length)
            return AshReadResult.Fail(AshReadError.BufferOverflow);

        if (!AshCtrl.TryParse(buffer[0], out var ctrl))
            return AshReadResult.Fail(AshReadError.InvalidControl);

        var data = new byte[length - 3];

        buffer.AsSpan(1, length - 3).CopyTo(data);

        var computedCrc = Crc16.CcittFalse(buffer.AsSpan(0, length - 2));
        var receivedCrc = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan(length - 2, 2));
        if (computedCrc != receivedCrc)
            return AshReadResult.Fail(AshReadError.InvalidCrc);

        switch (ctrl.Type)
        {
            case AshFrameType.Data:
                if (!data.Length.IsBetween(3, 128))
                    return AshReadResult.Fail(AshReadError.InvalidPayloadSize);
                break;
            case AshFrameType.Ack:
            case AshFrameType.Nak:
            case AshFrameType.Reset:
                if (data.Length > 0)
                    return AshReadResult.Fail(AshReadError.InvalidPayloadSize);
                break;
            case AshFrameType.ResetAck:
            case AshFrameType.Error:
                if (data.Length != 2)
                    return AshReadResult.Fail(AshReadError.InvalidPayloadSize);
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        if (ctrl.Type == AshFrameType.Data)
            AshRandom.Xor(data);

        if (Verbose)
        {
            Console.ForegroundColor = ConsoleColor.DarkYellow;
            Console.WriteLine("In " + DateTime.Now.ToString("O"));
            Console.WriteLine($"Ctrl: {ctrl}");
            Console.WriteLine($"Data: {BitConverter.ToString(data).Replace("-", " ")}");
            // Console.WriteLine($"Buffer: {BitConverter.ToString(buffer.AsSpan(0, length).ToArray()).Replace("-", " ")}");
            // Console.WriteLine($"Crc: {receivedCrc.ToString("X").Replace("-", " ")}");
            Console.WriteLine();
        }

        return AshReadResult.Success(new AshFrame(ctrl, data));
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
                case AshReservedByte.XOn:
                case AshReservedByte.XOff:
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
                b ^= AshConstants.EscapeBit;
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

    private async Task<int> ReadByteAsync(CancellationToken cancellationToken)
    {
        var array = new byte[1];
        var bytes = await stream.ReadAsync(array, 0, 1, cancellationToken);
        if (bytes == 0)
            return -1;
        return array[0];
    }
}