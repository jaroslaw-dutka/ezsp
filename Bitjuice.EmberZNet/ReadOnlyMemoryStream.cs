namespace Bitjuice.EmberZNet;

public class ReadOnlyMemoryStream : Stream
{
    private readonly ReadOnlyMemory<byte> root;
    private ReadOnlyMemory<byte> current;
    private long position;

    public override bool CanRead => true;
    public override bool CanSeek => false;
    public override bool CanWrite => true;
    public override long Length => root.Length;
    public override long Position
    {
        get => position;
        set => Seek(value, SeekOrigin.Begin);
    }

    public ReadOnlyMemoryStream(Memory<byte> memory)
    {
        root = memory;
        current = memory;
    }

    public ReadOnlyMemoryStream(ReadOnlyMemory<byte> memory)
    {
        root = memory;
        current = memory;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        current.Span.Slice(0, count).CopyTo(buffer.AsSpan(offset));
        current = current.Slice(count);
        position += count;
        return count;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                position = offset;
                current = root.Slice((int)position);
                break;
            case SeekOrigin.Current:
                position += offset;
                current = root.Slice((int)position);
                break;
            case SeekOrigin.End:
                position = Length - offset;
                current = root.Slice((int)position);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(origin), origin, null);
        }
        return position;
    }

    public override void SetLength(long value) => throw new InvalidOperationException();
    public override void Write(byte[] buffer, int offset, int count) => throw new InvalidOperationException();
    public override void Flush() => throw new InvalidOperationException();
}