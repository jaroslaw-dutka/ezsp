using Ezsp.Utils;

namespace Ezsp.Ash;

public class AshWriter
{
    private readonly byte escapeBit = 0x20;
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

    public async Task DiscardAsync(CancellationToken cancellationToken)
    {
        await stream.WriteByteAsync((byte)AshReservedByte.Cancel, cancellationToken);
    }

    public async Task WriteAsync(AshFrame frame, CancellationToken cancellationToken)
    {
        if (verbose)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Out " + DateTime.Now.ToString("O"));
            Console.WriteLine(frame);
        }

        BeginFrame(frame.Control.ToByte());
        if (frame.Data is not null)
            WriteData(frame.Data, frame.Control.Type == AshFrameType.Data);
        EndFrame();
        await stream.WriteAsync(buffer, 0, bufferIndex, cancellationToken);
    }

    private void BeginFrame(byte ctrl)
    {
        inputIndex = 0;
        bufferIndex = 0;
        crc = 0xffff;
        WriteByte(ctrl, true, false);
    }

    private void WriteData(IEnumerable<byte> data, bool byteStuff)
    {
        foreach (var b in data) 
            WriteByte(b, true, byteStuff);
    }

    private void EndFrame()
    {
        var crcBytes = BitConverter.GetBytes(crc);
        WriteByte(crcBytes[1], true, false);
        WriteByte(crcBytes[0], true, false);
        WriteByte((byte)AshReservedByte.Flag, false, false);
    }

    private void WriteByte(byte b, bool escape, bool byteStuff)
    {
        if (byteStuff)
            b ^= AshPseudorandom.Bytes[inputIndex++];

        crc = Crc16.CcittFalse(crc, b);

        if (escape && reservedBytes.Contains(b))
        {
            buffer[bufferIndex++] = (byte)AshReservedByte.Escape;
            b ^= escapeBit;
        }

        buffer[bufferIndex++] = b;
    }
}