namespace Bitjuice.EmberZNet.Ash;

public enum AshFrameError
{
    InvalidControl,
    InvalidCrc,
    InvalidPayloadSize,
    EndOfStream,
    MessageTooShort,
    BufferOverflow
}