namespace Bitjuice.EmberZNet.Ash;

public static class AshRandom
{
    private const int Size = 256;

    public static readonly byte[] Bytes = new byte[Size];

    static AshRandom()
    {
        Bytes[0] = 0x42;
        for (var i = 1; i < Size; i++)
        {
            Bytes[i] = (byte)(Bytes[i - 1] >> 1);
            if ((Bytes[i - 1] & 1) == 1)
                Bytes[i] = (byte)(Bytes[i] ^ 0xB8);
        }
    }

    public static void Xor(Span<byte> data)
    {
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)(data[i] ^ Bytes[i]);
    }
}