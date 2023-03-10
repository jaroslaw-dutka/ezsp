using Bitjuice.EmberZNet.Ash.Utils;

namespace Bitjuice.EmberZNet.Ash;

public class AshWriter : IAshWriter
{
    private readonly byte[] reservedBytes = Enum.GetValuesAsUnderlyingType<AshReservedByte>().OfType<byte>().ToArray();
    private readonly Stream stream;
    private readonly byte[] buffer;
    private int inputIndex;
    private int bufferIndex;
    private ushort crc;

    public bool Verbose { get; set; }

    public AshWriter(Stream stream, int bufferSize = AshConstants.MaxMessageSize+1)
    {
        this.stream = stream;
        buffer = new byte[bufferSize];
    }

    public async Task WriteDiscardAsync(CancellationToken cancellationToken) 
        => await stream.WriteAsync(new[] { (byte)AshReservedByte.Cancel }, cancellationToken);

    public async Task WriteAsync(byte ctrl, byte[] data, CancellationToken cancellationToken)
    {
        if (!AshCtrl.TryParse(ctrl, out var ctrlByte))
            throw new ArgumentException("Control byte is invalid", nameof(ctrl));

        if (Verbose)
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
            b ^= AshConstants.EscapeBit;
        }

        buffer[bufferIndex++] = b;
    }
}