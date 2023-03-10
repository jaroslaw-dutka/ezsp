namespace Bitjuice.EmberZNet.Ash;

public enum AshReservedByte: byte
{
    /// <summary>
    /// XOn: Resume transmission. Used in XON/XOFF flow control. Always ignored if received by the NCP.
    /// </summary>
    XOn = 0x11,

    /// <summary>
    /// XOff: Stop transmission. Used in XON/XOFF flow control. Always ignored if received by the NCP.
    /// </summary>
    XOff = 0x13,

    /// <summary>
    /// Substitute Byte: Replaces a byte received with a low - level communication error(e.g., framing error) from the UART.When a Substitute Byte is processed, the data between the previous and the next Flag Bytes is ignored.
    /// </summary>
    Substitute = 0x18,

    /// <summary>
    /// Cancel Byte: Terminates a frame in progress. A Cancel Byte causes all data received since the previous Flag Byte to be ignored. Note that as a special case, RST and RSTACK frames are preceded by Cancel Bytes to ignore any link startup noise.
    /// </summary>
    Cancel = 0x1A,

    /// <summary>
    /// Escape Byte: Indicates that the following byte is escaped.If the byte after the Escape Byte is not a reserved byte, bit 5 of the byte is complemented to restore its original value. If the byte after the Escape Byte is a reserved value, the Escape Byte has no effect.
    /// </summary>
    Escape = 0x7D,

    /// <summary>
    /// Flag Byte: Marks the end of a frame. When a Flag Byte is received, the data received since the last Flag Byte or Cancel Byte is tested to see whether it is a valid frame.
    /// </summary>
    Flag = 0x7E
}