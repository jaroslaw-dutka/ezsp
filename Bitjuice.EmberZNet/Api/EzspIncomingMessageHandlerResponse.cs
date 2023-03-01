using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspIncomingMessageHandlerResponse
{
    [FieldOrder(0)]
    public EmberIncomingMessageType Type { get; set; }

    [FieldOrder(1)]
    public EmberApsFrame ApsFrame { get; set; }

    [FieldOrder(2)]
    public byte LastHopLqi { get; set; }

    [FieldOrder(3)]
    public byte LastHopRssi { get; set; }

    [FieldOrder(4)]
    public ushort Sender { get; set; }

    [FieldOrder(5)]
    public byte BindingIndex { get; set; }

    [FieldOrder(6)]
    public byte AddressIndex { get; set; }

    [FieldOrder(7)]
    public byte MessageLength { get; set; }

    [FieldOrder(8)]
    [FieldLength(nameof(MessageLength))]
    public byte[] MessageContents { get; set; }
}