namespace Bitjuice.EmberZNet.Ash;

public class AshControlByteFactory
{
    public static byte Reset() => (byte)AshFrameType.Reset;
    public static byte Data(byte frmNum, byte ackNum, bool retry) => (byte)((frmNum & 0x07) << 4 | (retry ? 0x08 : 0x00) | (ackNum & 0x07));
    public static byte Ack(byte ackNum, bool notReady) => (byte)((byte)AshFrameType.Ack | (notReady ? 0x08 : 0x00) | (ackNum & 0x07));
    public static byte Nak(byte ackNum, bool notReady) => (byte)((byte)AshFrameType.Nak | (notReady ? 0x08 : 0x00) | (ackNum & 0x07));
}