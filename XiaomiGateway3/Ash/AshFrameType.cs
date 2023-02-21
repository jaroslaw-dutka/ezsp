namespace XiaomiGateway3.Ash;

public enum AshFrameType: byte
{
    Data = 0x00,
    Ack = 0x80,
    Nak = 0xA0,
    Rst = 0xC0,
    Rstack = 0xC1,
    Error = 0xC2
}