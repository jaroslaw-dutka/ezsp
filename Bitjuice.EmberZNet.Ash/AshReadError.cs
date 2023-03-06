namespace Bitjuice.EmberZNet.Ash;

public enum AshReadError
{
    InvalidControl,
    InvalidCrc,
    InvalidPayloadSize,
    EndOfStream,
    MessageTooShort,
    BufferOverflow
}