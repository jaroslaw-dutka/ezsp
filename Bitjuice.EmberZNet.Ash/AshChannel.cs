﻿using System.Collections.Concurrent;
using Bitjuice.EmberZNet.Ash.Utils;

namespace Bitjuice.EmberZNet.Ash;

public class AshChannel
{
    private readonly ConcurrentQueue<AshDataSendTask> sendQueue = new();

    private readonly AshReader reader;
    private readonly AshWriter writer;

    private CancellationTokenSource? cts;
    private byte outgoingFrame;
    private byte outgoingFrameAck;
    private byte incomingFrame;
    private sbyte ackPending;

    public Action<byte[]>? DataReceived { get; set; }

    public AshChannel(Stream stream)
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

    public Task SendAsync(byte[] data)
    {
        var tcs = new TaskCompletionSource();
        sendQueue.Enqueue(new AshDataSendTask { Data = data, Tcs = tcs });
        return tcs.Task;
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
            else if (outgoingFrame != outgoingFrameAck || !sendQueue.TryDequeue(out var item))
            {
                try
                {
                    await Task.Delay(1, cancellationToken);
                }
                catch (TaskCanceledException)
                {
                    // ignore
                }
            }
            else
            {
                await writer.WriteDataAsync(outgoingFrame, incomingFrame, false, item.Data, cancellationToken);
                outgoingFrame = outgoingFrame.IncMod8();
                item.Tcs.SetResult();
            }
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
                if (frame.Control.AckNumber == outgoingFrame)
                {
                    outgoingFrameAck = frame.Control.AckNumber;
                    // incomingFrame = frame.Control.FrameNumber.IncMod8();
                }
                else
                    ackPending = -1;
            }

            if (frame.Control.Type == AshFrameType.Nak)
            {
                if (frame.Control.AckNumber == outgoingFrame)
                {
                    // TODO: handle NAK
                }
                else
                    ackPending = -1;
            }

            if (frame.Control.Type == AshFrameType.Data)
            {
                if (frame.Control.FrameNumber != incomingFrame)
                {
                    ackPending = -1;
                }
                else
                {
                    outgoingFrameAck = frame.Control.AckNumber;
                    ackPending = 1;
                    incomingFrame = frame.Control.FrameNumber.IncMod8();
                    DataReceived?.Invoke(frame.Data);
                }
            }
        }
    }
}