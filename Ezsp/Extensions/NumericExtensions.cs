namespace Ezsp.Extensions
{
    public static class NumericExtensions
    {
        public static bool IsBetween<T>(this T value, T low, T high) where T: IComparable<T>
        {
            return value.CompareTo(low) >= 0 && value.CompareTo(high) <= 0;
        }

        public static byte IncMod8(this byte value)
        {
            return (byte)((value + 1) % 8);
        }
    }
}
