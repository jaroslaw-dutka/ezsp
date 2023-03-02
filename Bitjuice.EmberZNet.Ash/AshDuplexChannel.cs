using System.Collections.Concurrent;
using Bitjuice.EmberZNet.Ash.Utils;

namespace Bitjuice.EmberZNet.Ash;

public class AshDuplexChannel
{
    private readonly ConcurrentQueue<AshDataSendTask> sendQueue = new();
    private AshDataSendTask?[] ackQueue = new AshDataSendTask?[8];

    private readonly AshReader reader;
    private readonly AshWriter writer;

    private byte txWindow = 1;

    private CancellationTokenSource? cts;
    private byte outgoingFrame;
    private byte outgoingFrameAck;
    private byte incomingFrame;
    private byte incomingFrameAck;
    private sbyte ackPending;

    public Action<byte[]>? DataReceived { get; set; }

    public AshDuplexChannel(Stream stream)
    {
        reader = new AshReader(stream, 256, true);
        writer = new AshWriter(stream, 256, true);
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        await DisconnectAsync(cancellationToken);

        await writer.WriteDiscardAsync(cancellationToken);
        await writer.WriteResetAsync(cancellationToken);

        var frame = await reader.ReadAsync(cancellationToken);
        while (!frame.IsValid || frame.Control.Type != AshFrameType.ResetAck) 
            frame = await reader.ReadAsync(cancellationToken);

        cts = new CancellationTokenSource();

        Task.Run(async () => await SendLoop(cts.Token), cts.Token);
        Task.Run(async () => await ReadLoop(cts.Token), cts.Token);
    }

    public Task DisconnectAsync(CancellationToken cancellationToken)
    {
        cts?.Cancel();
        cts = null;

        outgoingFrame = 0;
        incomingFrame = 0;
        ackPending = 0;

        return Task.CompletedTask;
    }

    public void AddToQueue(byte[] data)
    {
        sendQueue.Enqueue(new AshDataSendTask { Data = data });
    }

    private async Task SendLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (ackPending != 0)
            {
                if (ackPending > 0)
                    await writer.WriteAckAsync(incomingFrame, cancellationToken);
                else
                    await writer.WriteNakAsync(incomingFrame, cancellationToken);
                ackPending = 0;
            }

            // TODO: priority queue?
            for (var i = outgoingFrameAck; i != outgoingFrame; i = i.IncMod8())
            {
                var resendItem = ackQueue[i];
                if (resendItem is null)
                    continue;
                if (resendItem.Retries > 3)
                    throw new Exception("dupa");
                if (DateTime.UtcNow - resendItem.SendTime <= TimeSpan.FromMilliseconds(100)) 
                    continue;
                await writer.WriteDataAsync(i, incomingFrame, true, resendItem.Data, cancellationToken);
                resendItem.Retries++;
                resendItem.SendTime = DateTime.UtcNow;
            }

            if (outgoingFrame.SubMod8(outgoingFrameAck) >= txWindow)
            {
                await Wait(100, cancellationToken);
                continue;
            }

            if (!sendQueue.TryDequeue(out var newItem))
            {
                await Wait(100, cancellationToken);
                continue;
            }

            await writer.WriteDataAsync(outgoingFrame, incomingFrame, false, newItem.Data, cancellationToken);

            newItem.Retries++;
            newItem.SendTime = DateTime.UtcNow;

            ackQueue[outgoingFrame] = newItem;
            outgoingFrame = outgoingFrame.IncMod8();
        }
    }

    private async Task ReadLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var frame = await reader.ReadAsync(cancellationToken);
            if (!frame.IsValid)
            {
                ackPending = -1;
                continue;
            }

            if (frame.Control.Type == AshFrameType.Error)
                throw new AshException(frame.Data[0], frame.Data[1]);

            if (frame.Control.Type == AshFrameType.Ack)
            {
                if (frame.Control.AckNumber.InRangeMod8(outgoingFrameAck, outgoingFrame))
                    outgoingFrameAck = frame.Control.AckNumber;
            }

            if (frame.Control.Type == AshFrameType.Nak)
            {
                if (frame.Control.AckNumber.InRangeMod8(outgoingFrameAck, outgoingFrame))
                {
                    // TODO: handle NAK
                }
            }

            if (frame.Control.Type == AshFrameType.Data)
            {
                if (!frame.Control.AckNumber.InRangeMod8(outgoingFrameAck, outgoingFrame))
                {
                    ackPending = -1;
                }
                else
                {
                    ackPending = 1;
                    incomingFrame = frame.Control.FrameNumber.IncMod8();
                    outgoingFrameAck = frame.Control.AckNumber;
                    DataReceived?.Invoke(frame.Data);
                }
            }
        }
    }

    private static async Task Wait(int delay, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(delay, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // ignore
        }
    }
}