﻿namespace XiaomiGateway3.Ash;

public static class AshRandom
{
    private static readonly byte[] Random;

    static AshRandom()
    {
        var size = 256;
        Random = new byte[size];
        Random[0] = 0x42;
        for (var i = 1; i < size; i++)
        {
            Random[i] = (byte)(Random[i - 1] >> 1);
            if ((Random[i - 1] & 1) == 1)
                Random[i] = (byte)(Random[i] ^ 0xB8);
        }
    }

    public static void Replace(Span<byte> data)
    {
        for (var i = 0; i < data.Length; i++)
            data[i] = (byte)(data[i] ^ Random[i]);
    }
}