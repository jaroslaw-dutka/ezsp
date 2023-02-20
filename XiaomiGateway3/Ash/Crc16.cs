public class Crc16
{
    public static ushort CcittFalse(Span<byte> data)
    {
        return CcittFalse(0xFFFF, data);
    }

    public static ushort CcittFalse(ushort crc, Span<byte> data)
    {
        for (int i = 0; i < data.Length; i++)
        {
            crc ^= (ushort)(data[i] << 8);
            for (int j = 0; j < 8; j++)
            {
                if ((crc & 0x8000) > 0)
                    crc = (ushort)((crc << 1) ^ 0x1021);
                else
                    crc <<= 1;
            }
        }
        return crc;
    }
}