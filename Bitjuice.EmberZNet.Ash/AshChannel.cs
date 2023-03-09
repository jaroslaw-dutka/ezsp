using System.Collections.Concurrent;
using Bitjuice.EmberZNet.Ash.Utils;

namespace Bitjuice.EmberZNet.Ash;

public class AshChannel : IAshChannel
{
    private const byte OutgoingWindow = 1;

    private readonly AshReader reader;
    private readonly AshWriter writer;

    private AshChannelStatus status;
    private Task? sendTask;
    private Task? readTask;

    private CancellationTokenSource? cts;

    private readonly ConcurrentQueue<AshSendTask> inputQueue = new();
    private readonly AshSendTask?[] sendQueue = new AshSendTask?[8];

    private byte incomingFrameNext;
    private byte outgoingFrameNext;
    private byte outgoingFrameAck;
    private bool ackPending;
    private bool rejectCondition;

    public AshChannel(Stream stream)
    {
        reader = new AshReader(stream, 256, true);
        writer = new AshWriter(stream, 256, true);
        status = AshChannelStatus.NotConnected;
    }

    public async Task ConnectAsync(IAshDataHandler handler, CancellationToken cancellationToken)
    {
        ExpectStatus(AshChannelStatus.NotConnected);
        status = AshChannelStatus.Connecting;

        ResetState();

        await writer.WriteDiscardAsync(cancellationToken);
        await writer.WriteResetAsync(cancellationToken);
        var result = await reader.ReadAsync(cancellationToken);
        while (result.Error is not null || result.Frame is null || result.Frame.Ctrl.Type != AshFrameType.ResetAck)
            result = await reader.ReadAsync(cancellationToken); // Discard any invalid or not ResetAck frames

        cts = new CancellationTokenSource();
        #pragma warning disable CS4014
        sendTask = Task.Run(async () => await SendLoop(cts.Token), cts.Token);
        readTask = Task.Run(async () => await ReadLoop(handler, cts.Token), cts.Token);
        #pragma warning restore CS4014

        status = AshChannelStatus.Connected;
    }

    public async Task DisconnectAsync()
    {
        ExpectStatus(AshChannelStatus.Connected);
        status = AshChannelStatus.Disconnecting;

        cts?.Cancel();
        cts = null;

        await Task.WhenAll(sendTask ?? Task.CompletedTask, readTask ?? Task.CompletedTask);

        ResetState();

        status = AshChannelStatus.NotConnected;
    }

    public void Send(byte[] data)
    {
        inputQueue.Enqueue(new AshSendTask(data));
    }

    private async Task SendLoop(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
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
                    var resendItem = sendQueue[i];
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
                    await Task.Delay(100, cancellationToken);
                    continue;
                }

                if (outgoingFrameNext.SubMod8(outgoingFrameAck) >= OutgoingWindow)
                {
                    await Task.Delay(100, cancellationToken);
                    continue;
                }

                if (!inputQueue.TryDequeue(out var newItem))
                {
                    await Task.Delay(100, cancellationToken);
                    continue;
                }

                await writer.WriteDataAsync(outgoingFrameNext, incomingFrameNext, false, newItem.Data, cancellationToken);

                newItem.MarkAsSent();

                sendQueue[outgoingFrameNext] = newItem;
                outgoingFrameNext = outgoingFrameNext.IncMod8();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private async Task ReadLoop(IAshDataHandler handler, CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var result = await reader.ReadAsync(cancellationToken);
                if (result.Error is not null || result.Frame is null)
                {
                    Console.WriteLine("Invalid frame: " + result.Error);
                    while (ackPending)
                        await Task.Delay(10, cancellationToken);
                    rejectCondition = true;
                    ackPending = true;
                    continue;
                }

                var frame = result.Frame;

                if (frame.Ctrl.Type == AshFrameType.Error)
                    throw new AshException(frame.Data[0], frame.Data[1]);

                if (frame.Ctrl.Type == AshFrameType.Ack)
                {
                    if (frame.Ctrl.AckNumber.InRangeMod8(outgoingFrameAck, outgoingFrameNext))
                    {
                        outgoingFrameAck = frame.Ctrl.AckNumber;
                    }
                }

                if (frame.Ctrl.Type == AshFrameType.Nak)
                {
                    if (frame.Ctrl.AckNumber.InRangeMod8(outgoingFrameAck, outgoingFrameNext))
                    {
                        for (var i = frame.Ctrl.AckNumber.IncMod8(); i != outgoingFrameNext; i = i.IncMod8())
                            sendQueue[i]?.MarkAsRejected();
                    }
                }

                if (frame.Ctrl.Type == AshFrameType.Data)
                {
                    if (frame.Ctrl.FrameNumber == incomingFrameNext && frame.Ctrl.AckNumber.InRangeMod8(outgoingFrameAck, outgoingFrameNext))
                    {
                        sendQueue[frame.Ctrl.FrameNumber] = null;

                        incomingFrameNext = frame.Ctrl.FrameNumber.IncMod8();
                        outgoingFrameAck = frame.Ctrl.AckNumber;

                        rejectCondition = false;
                        ackPending = true;

                        Task.Run(async () => { await handler.HandleAsync(frame.Data); });
                    }
                    else
                    {
                        if (!frame.Ctrl.Retransmission)
                        {
                            while (ackPending)
                                await Task.Delay(10, cancellationToken);
                            if (!rejectCondition)
                                ackPending = true;
                            rejectCondition = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private void ExpectStatus(AshChannelStatus expected)
    {
        if (status != expected)
            throw new InvalidOperationException($"Invalid channel status. Expected: {expected}, Got: {status}.");
    }

    private void ResetState()
    {
        incomingFrameNext = 0;
        outgoingFrameNext = 0;
        outgoingFrameAck = 0;
        inputQueue.Clear();
        ackPending = false;
        rejectCondition = false;
        for (var i = 0; i < sendQueue.Length; i++)
            sendQueue[i] = null;
    }
}