namespace Ezsp.Ash;

public static class AshPseudorandom
{
    public static readonly byte[] Bytes;

    static AshPseudorandom()
    {
        var size = 256;
        Bytes = new byte[size];
        Bytes[0] = 0x42;
        for (var i = 1; i < size; i++)
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