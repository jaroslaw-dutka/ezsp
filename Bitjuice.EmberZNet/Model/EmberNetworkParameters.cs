using BinarySerialization;

namespace Bitjuice.EmberZNet.Model;

public class EmberNetworkParameters
{
    [FieldOrder(0)]
    public EzspExtendedPanId ExtendedPanId { get; set; }

    [FieldOrder(1)]
    public ushort PanId { get; set; }

    [FieldOrder(2)]
    public byte RadioTxPower { get; set; }

    [FieldOrder(3)]
    public byte RadioChannel { get; set; }

    [FieldOrder(4)]
    public EmberJoinMethod JoinMethod { get; set; }

    [FieldOrder(5)]
    public ushort NwkManagerId { get; set; }

    [FieldOrder(6)]
    public byte NwkUpdateId { get; set; }

    [FieldOrder(7)]
    public uint Channels { get; set; }
}