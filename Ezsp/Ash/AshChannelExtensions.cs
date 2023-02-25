namespace Ezsp.Ash;

public static class AshChannelExtensions
{
    public static async Task WriteResetAsync(this AshWriter channel, CancellationToken cancellationToken)
    {
        await channel.WriteAsync(new AshFrame
        {
            Control = new AshControlByte
            {
                Type = AshFrameType.Rst
            }
        }, cancellationToken);
    }

    public static async Task WriteDataAsync(this AshWriter channel, byte frmNumber, byte ackNumber, byte[] data, CancellationToken cancellationToken)
    {
        await channel.WriteAsync(new AshFrame
        {
            Control = new AshControlByte
            {
                Type = AshFrameType.Data,
                FrameNumber = frmNumber,
                AckNumber = ackNumber,
            },
            Data = data
        }, cancellationToken);
    }

    public static async Task WriteAckAsync(this AshWriter channel, byte ackNumber, CancellationToken cancellationToken)
    {
        await channel.WriteAsync(new AshFrame
        {
            Control = new AshControlByte
            {
                Type = AshFrameType.Ack,
                AckNumber = ackNumber,
            }
        }, cancellationToken);
    }

    public static async Task WriteNakAsync(this AshWriter channel, byte ackNumber, CancellationToken cancellationToken)
    {
        await channel.WriteAsync(new AshFrame
        {
            Control = new AshControlByte
            {
                Type = AshFrameType.Nak,
                AckNumber = ackNumber,
            }
        }, cancellationToken);
    }
}