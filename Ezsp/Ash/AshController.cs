using System.Collections.Concurrent;
using Ezsp.Extensions;

namespace Ezsp.Ash;

public class AshController
{
    private readonly AshClient client;
    private readonly ConcurrentQueue<AshDataSendTask> dataQueue = new();
    private readonly ConcurrentDictionary<byte, TaskCompletionSource<byte[]>> responseQueue = new();

    private CancellationTokenSource? cts;

    private byte outgoingFrame;
    private byte incomingFrame;
    private sbyte ackPending;

    public AshController(AshClient client)
    {
        this.client = client;
    }

    public void Connect()
    {
        cts?.Cancel();

        client.WriteDiscard();
        client.WriteReset();

        var frame = client.Read();
        while (frame.Control.Type != AshFrameType.Rstack)
            frame = client.Read();

        cts = new CancellationTokenSource();
        Task.Run(() => SendLoop(cts.Token), cts.Token);
        Task.Run(() => ReadLoop(cts.Token), cts.Token);
    }

    public void Disconnect()
    {
        cts?.Cancel(true);
    }

    public Task<byte[]> SendSync(byte[] data)
    {
        var tcs = new TaskCompletionSource<byte[]>();
        dataQueue.Enqueue(new AshDataSendTask { Data = data, Tcs = tcs });
        return tcs.Task;
    }

    private async Task SendLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            if (ackPending > 0)
            {
                client.WriteAck(incomingFrame);
                ackPending = 0;
            }
            else if (ackPending < 0)
            {
                client.WriteNak(incomingFrame);
                ackPending = 0;
            }
            else if (responseQueue.Count > 1 || !dataQueue.TryDequeue(out var item))
            {
                await Task.Delay(100, cancellationToken);
            }
            else
            {
                client.WriteData(outgoingFrame, incomingFrame, item.Data);
                responseQueue.TryAdd(outgoingFrame, item.Tcs);
                outgoingFrame = outgoingFrame.IncMod8();
            }
        }
    }

    private async Task ReadLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var frame = client.Read();
            if (frame == null)
            {
                ackPending = -1;
                continue;
            }

            if (frame.Control.Type == AshFrameType.Error)
                throw new AshException(frame.Data[0], frame.Data[1]);

            if (frame.Control.Type == AshFrameType.Ack)
            {
                incomingFrame = frame.Control.AckNumber;
                if (responseQueue.TryRemove(frame.Control.AckNumber, out var tcs))
                    tcs.TrySetResult(null);
            }

            if (frame.Control.Type == AshFrameType.Nak)
            {
                incomingFrame = frame.Control.AckNumber;
                if (responseQueue.TryRemove(frame.Control.AckNumber, out var tcs))
                    tcs.TrySetResult(null);
            }

            if (frame.Control.Type == AshFrameType.Data)
            {
                if (frame.Control.FrameNumber != incomingFrame)
                {
                    ackPending = -1;
                }
                else
                {
                    incomingFrame = frame.Control.FrameNumber.IncMod8();
                    if (responseQueue.TryRemove(frame.Control.FrameNumber, out var tcs))
                        tcs.TrySetResult(frame.Data);
                    ackPending = 1;
                }
            }
        }
    }
}