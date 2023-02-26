using System.Collections.Concurrent;
using Ezsp.Utils;

namespace Ezsp.Ash;

public class AshClient
{
    private readonly AshReader reader;
    private readonly AshWriter writer;

    private readonly ConcurrentQueue<AshDataSendTask> dataQueue = new();
    private readonly ConcurrentDictionary<byte, TaskCompletionSource<byte[]>> responseQueue = new();

    private CancellationTokenSource? cts;

    private byte outgoingFrame;
    private byte incomingFrame;
    private sbyte ackPending;

    public Action<byte[]> OnMessage { get; set; }

    public AshClient(Stream stream)
    {
        reader = new AshReader(stream, 256, true);
        writer = new AshWriter(stream, 256, true);
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        cts?.Cancel();

        await writer.DiscardAsync(cancellationToken);
        await writer.WriteResetAsync(cancellationToken);

        var frame = await reader.ReadAsync(cancellationToken);
        while (frame?.Control.Type != AshFrameType.Rstack)
            frame = await reader.ReadAsync(cancellationToken);

        cts = new CancellationTokenSource();
        Task.Run(async () => await SendLoop(cts.Token), cts.Token);
        Task.Run(async () => await ReadLoop(cts.Token), cts.Token);
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
                else if (!responseQueue.IsEmpty || !dataQueue.TryDequeue(out var item))
                {
                    await Task.Delay(100, cancellationToken);
                }
                else
                {
                    await writer.WriteDataAsync(outgoingFrame, incomingFrame, item.Data, cancellationToken);
                    outgoingFrame = outgoingFrame.IncMod8();
                    item.Tcs.SetResult(Array.Empty<byte>());
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
                    if (frame.Control.AckNumber != outgoingFrame)
                    {
                        ackPending = -1;
                    }
                }

                if (frame.Control.Type == AshFrameType.Nak)
                {
                    if (frame.Control.AckNumber != outgoingFrame)
                    {
                        ackPending = -1;
                    }

                    // TODO: handle NAK
                }

                if (frame.Control.Type == AshFrameType.Data)
                {
                    if (frame.Control.FrameNumber != incomingFrame)
                    {
                        ackPending = -1;
                    }
                    else
                    {
                        ackPending = 1;
                        incomingFrame = frame.Control.FrameNumber.IncMod8();
                        OnMessage?.Invoke(frame.Data);
                    }
                }
            }
        }
        catch (TaskCanceledException)
        {
        }
    }
}