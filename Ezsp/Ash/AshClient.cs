using System.Collections.Concurrent;
using Ezsp.Utils;

namespace Ezsp.Ash;

public class AshClient
{
    private readonly AshReader reader;
    private readonly AshWriter writer;

    private readonly ConcurrentQueue<AshDataSendTask> dataQueue = new();
    private readonly ConcurrentQueue<byte[]> inQueue = new();
    private readonly ConcurrentDictionary<byte, TaskCompletionSource<byte[]>> responseQueue = new();

    private CancellationTokenSource? cts;

    private byte outgoingFrame;
    private byte incomingFrame;
    private sbyte ackPending;
    
    public AshClient(Stream stream)
    {
        reader = new AshReader(stream, 256, true);
        writer = new AshWriter(stream, 256, true);
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        if (cts is not null)
        {
            cts.Cancel();
        }

        await writer.DiscardAsync(cancellationToken);
        await writer.WriteResetAsync(cancellationToken);

        var frame = await reader.ReadAsync(cancellationToken);
        while (frame.Control.Type != AshFrameType.Rstack)
            frame = await reader.ReadAsync(cancellationToken);

        cts = new CancellationTokenSource();
        Task.Run(async () => await SendLoop(cts.Token), cts.Token);
        Task.Run(async () => await ReadLoop(cts.Token), cts.Token);
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
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                if (ackPending > 0)
                {
                    await writer.WriteAckAsync(incomingFrame, cancellationToken);
                    ackPending = 0;
                }
                else if (ackPending < 0)
                {
                    await writer.WriteNakAsync(incomingFrame, cancellationToken);
                    ackPending = 0;
                }
                else if (responseQueue.Count > 0 || !dataQueue.TryDequeue(out var item))
                {
                    await Task.Delay(100, cancellationToken);
                }
                else
                {
                    await writer.WriteDataAsync(outgoingFrame, incomingFrame, item.Data, cancellationToken);
                    responseQueue.TryAdd(outgoingFrame, item.Tcs);
                    outgoingFrame = outgoingFrame.IncMod8();
                }
            }
        }
        catch (TaskCanceledException)
        {
        }
    }

    private async Task ReadLoop(CancellationToken cancellationToken)
    {
        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var frame = await reader.ReadAsync(cancellationToken);
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
        catch (TaskCanceledException)
        {
        }
    }
}