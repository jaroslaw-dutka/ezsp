using Bitjuice.EmberZNet.Ash.Utils;

namespace Bitjuice.EmberZNet.Ash;

public class AshWriter
{
    private const byte EscapeBit = 0x20;

    private readonly byte[] reservedBytes = Enum.GetValuesAsUnderlyingType<AshReservedByte>().OfType<byte>().ToArray();
    private readonly Stream stream;
    private readonly bool verbose;
    private readonly byte[] buffer;
    private int inputIndex;
    private int bufferIndex;
    private ushort crc;

    public AshWriter(Stream stream, int bufferSize = 256, bool verbose = false)
    {
        this.stream = stream;
        this.verbose = verbose;
        buffer = new byte[bufferSize];
    }

    public async Task WriteDiscardAsync(CancellationToken cancellationToken) 
        => await stream.WriteAsync(new[] { (byte)AshReservedByte.Cancel }, cancellationToken);

    public async Task WriteResetAsync(CancellationToken cancellationToken) 
        => await WriteAsync(AshControlByteFactory.Reset(), Array.Empty<byte>(), cancellationToken);

    public async Task WriteDataAsync(byte frmNumber, byte ackNumber, bool retry, byte[] data, CancellationToken cancellationToken) 
        => await WriteAsync(AshControlByteFactory.Data(frmNumber, ackNumber, retry), data, cancellationToken);

    public async Task WriteAckAsync(byte ackNumber, CancellationToken cancellationToken) 
        => await WriteAsync(AshControlByteFactory.Ack(ackNumber, false), Array.Empty<byte>(), cancellationToken);

    public async Task WriteNakAsync(byte ackNumber, CancellationToken cancellationToken) 
        => await WriteAsync(AshControlByteFactory.Nak(ackNumber, false), Array.Empty<byte>(), cancellationToken);

    private async Task WriteAsync(byte ctrl, byte[] data, CancellationToken cancellationToken)
    {
        if (!AshControlByte.TryParse(ctrl, out var ctrlByte))
            throw new ArgumentException("Control byte is invalid", nameof(ctrl));

        if (verbose)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Out " + DateTime.Now.ToString("O"));
            Console.WriteLine($"Ctrl: {ctrlByte}");
            Console.WriteLine($"Data: {BitConverter.ToString(data).Replace("-", " ")}");
            Console.WriteLine();
        }

        inputIndex = 0;
        bufferIndex = 0;
        crc = 0xffff;
        WriteByte(ctrl, true, false);
        foreach (var b in data)
            WriteByte(b, true, true);
        var crcBytes = BitConverter.GetBytes(crc);
        WriteByte(crcBytes[1], true, false);
        WriteByte(crcBytes[0], true, false);
        WriteByte((byte)AshReservedByte.Flag, false, false);
        await stream.WriteAsync(buffer, 0, bufferIndex, cancellationToken);
    }

    private void WriteByte(byte b, bool escape, bool stuff)
    {
        if (stuff)
            b ^= AshRandom.Bytes[inputIndex++];

        crc = Crc16.CcittFalse(crc, b);

        if (escape && reservedBytes.Contains(b))
        {
            buffer[bufferIndex++] = (byte)AshReservedByte.Escape;
            b ^= EscapeBit;
        }

        buffer[bufferIndex++] = b;
    }
}