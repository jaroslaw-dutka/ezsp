using System.Collections.Concurrent;
using Bitjuice.EmberZNet.Ash.Utils;

namespace Bitjuice.EmberZNet.Ash;

public class AshDuplexChannel
{
    private const byte OutgoingWindow = 1;

    private readonly AshReader reader;
    private readonly AshWriter writer;

    private readonly IAshDataHandler handler;

    private CancellationTokenSource? cts;

    private readonly ConcurrentQueue<AshSendDataTask> sendQueue = new();
    private readonly AshSendDataTask?[] ackQueue = new AshSendDataTask?[8];

    private byte incomingFrameNext;
    private byte outgoingFrameNext;
    private byte outgoingFrameAck;
    private bool ackPending;
    private bool rejectCondition;

    public Action<byte[]>? OnDataReceived { get; set; }

    public AshDuplexChannel(Stream stream, IAshDataHandler handler)
    {
        reader = new AshReader(stream, 256, true);
        writer = new AshWriter(stream, 256, true);
        this.handler = handler;
    }

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        Reset();

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
        Reset();
        return Task.CompletedTask;
    }

    private void Reset()
    {
        cts?.Cancel();
        cts = null;
        incomingFrameNext = 0;
        outgoingFrameNext = 0;
        outgoingFrameAck = 0;
        sendQueue.Clear();
        ackPending = false;
        rejectCondition = false;
        for (var i = 0; i < ackQueue.Length; i++)
            ackQueue[i] = null;
    }

    public void SendQueue(byte[] data)
    {
        sendQueue.Enqueue(new AshSendDataTask(data));
    }

    private async Task SendLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            while (ackPending)
            {
                if (rejectCondition)
                    await writer.WriteNakAsync(incomingFrameNext, cancellationToken);
                else
                    await writer.WriteAckAsync(incomingFrameNext, cancellationToken);

                ackPending = false;
            }

            for (var i = outgoingFrameAck; i != outgoingFrameNext; i = i.IncMod8())
            {
                var resendItem = ackQueue[i];
                if (resendItem is null)
                    continue;
                if (resendItem.Retries > 3)
                    throw new Exception("Error");
                if (!resendItem.ShouldBeResend()) 
                    continue;
                await writer.WriteDataAsync(i, incomingFrameNext, true, resendItem.Data, cancellationToken);
                resendItem.MarkAsSent();
            }

            if (rejectCondition)
            {
                Console.WriteLine("Reject condition");
                await Wait(100, cancellationToken);
                continue;
            }

            if (outgoingFrameNext.SubMod8(outgoingFrameAck) >= OutgoingWindow)
            {
                await Wait(100, cancellationToken);
                continue;
            }

            if (!sendQueue.TryDequeue(out var newItem))
            {
                await Wait(100, cancellationToken);
                continue;
            }

            await writer.WriteDataAsync(outgoingFrameNext, incomingFrameNext, false, newItem.Data, cancellationToken);

            newItem.MarkAsSent();

            ackQueue[outgoingFrameNext] = newItem;
            outgoingFrameNext = outgoingFrameNext.IncMod8();
        }
    }

    private async Task ReadLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            var frame = await reader.ReadAsync(cancellationToken);
            if (!frame.IsValid)
            {
                Console.WriteLine("Invalid frame: " + frame.Error);
                rejectCondition = true;
                ackPending = true;
                continue;
            }

            if (frame.Control.Type == AshFrameType.Error)
                throw new AshException(frame.Data[0], frame.Data[1]);

            if (frame.Control.Type == AshFrameType.Ack)
            {
                if (frame.Control.AckNumber.InRangeMod8(outgoingFrameAck, outgoingFrameNext))
                {
                    outgoingFrameAck = frame.Control.AckNumber;
                }
            }

            if (frame.Control.Type == AshFrameType.Nak)
            {
                if (frame.Control.AckNumber.InRangeMod8(outgoingFrameAck, outgoingFrameNext))
                {
                    for (var i = frame.Control.AckNumber.IncMod8(); i != outgoingFrameNext; i = i.IncMod8()) 
                        ackQueue[i]?.MarkAsNotAccepted();
                }
            }

            if (frame.Control.Type == AshFrameType.Data)
            {
                // if (frame.Control.Retransmission && frame.Control.AckNumber.InRangeMod8(outgoingFrameAck, outgoingFrameNext))
                // {
                //     // TODO
                //     throw new NotImplementedException();
                // }
                // else 
                if (frame.Control.FrameNumber != incomingFrameNext || !frame.Control.AckNumber.InRangeMod8(outgoingFrameAck, outgoingFrameNext))
                {
                    if (!rejectCondition) 
                        ackPending = true;
                    rejectCondition = true;
                }
                else
                {
                    ackQueue[incomingFrameNext] = null;

                    incomingFrameNext = incomingFrameNext.IncMod8();
                    outgoingFrameAck = frame.Control.AckNumber;

                    rejectCondition = false;
                    ackPending = true;

                    Task.Run(async () => { await handler.HandleAsync(frame.Data); });
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