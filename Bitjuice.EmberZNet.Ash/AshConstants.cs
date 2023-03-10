namespace Bitjuice.EmberZNet.Ash;

public class AshConstants
{
    public const byte EscapeBit = 0x20;
    public const byte MaxMessageSize = 1 + 2 + 128; // Ctrl + Crc + EzspFrame
}