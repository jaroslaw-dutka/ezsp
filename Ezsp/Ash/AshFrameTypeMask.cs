namespace Ezsp.Ash;

public enum AshFrameTypeMask : byte
{
    Data = 0x80,
    Ack = 0xE0,
    Nak = 0xE0,
    Rst = 0xFF,
    Rstack = 0xFF,
    Error = 0xFF
}