namespace Bitjuice.EmberZNet.Ash.Utils;

public static class NumericExtensions
{
    public static bool IsBetween<T>(this T value, T low, T high) where T : IComparable<T>
    {
        return value.CompareTo(low) >= 0 && value.CompareTo(high) <= 0;
    }

    public static byte IncMod8(this byte value)
    {
        return (byte)((value + 1) % 8);
    }

    public static bool InRangeMod8(this byte value, byte low, byte high)
    {
        if (low < high)
            return value >= low && value <= high;
        else
            return value >= high || value <= low;
    }

    public static byte SubMod8(this byte value1, byte value2)
    {
        if (value1 < value2)
            return (byte)(value1 + 8 - value2);
        else
            return (byte)(value1 - value2);
    }
}