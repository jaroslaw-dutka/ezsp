namespace Bitjuice.EmberZNet.Ash;

public enum AshFrameError
{
    InvalidControl,
    InvalidCrc,
    InvalidSize,
    EndOfStream,
    BufferOverflow
}