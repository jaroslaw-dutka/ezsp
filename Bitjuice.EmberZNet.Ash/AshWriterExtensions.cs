namespace Bitjuice.EmberZNet.Ash;

public static class AshWriterExtensions
{
    public static async Task WriteResetAsync(this IAshWriter writer, CancellationToken cancellationToken)
        => await writer.WriteAsync(AshCtrl.Reset(), Array.Empty<byte>(), cancellationToken);

    public static async Task WriteDataAsync(this IAshWriter writer, byte frmNumber, byte ackNumber, bool retry, byte[] data, CancellationToken cancellationToken)
        => await writer.WriteAsync(AshCtrl.Data(frmNumber, ackNumber, retry), data, cancellationToken);

    public static async Task WriteAckAsync(this IAshWriter writer, byte ackNumber, CancellationToken cancellationToken)
        => await writer.WriteAsync(AshCtrl.Ack(ackNumber, false), Array.Empty<byte>(), cancellationToken);

    public static async Task WriteNakAsync(this IAshWriter writer, byte ackNumber, CancellationToken cancellationToken)
        => await writer.WriteAsync(AshCtrl.Nak(ackNumber, false), Array.Empty<byte>(), cancellationToken);
}