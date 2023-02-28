namespace Bitjuice.EmberZNet.Ash;

public enum AshFrameType: byte
{
    Data = 0x00,
    Ack = 0x80,
    Nak = 0xA0,
    Reset = 0xC0,
    ResetAck = 0xC1,
    Error = 0xC2
}