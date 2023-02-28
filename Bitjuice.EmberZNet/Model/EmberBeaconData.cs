using BinarySerialization;

namespace Bitjuice.EmberZNet.Model;

public class EmberBeaconData
{
    [FieldOrder(0)]
    public byte Channel { get; set; }

    [FieldOrder(1)]
    public byte Lqi { get; set; }

    [FieldOrder(2)]
    public sbyte Rssi { get; set; }

    [FieldOrder(3)]
    public byte Depth { get; set; }

    [FieldOrder(4)]
    public byte NwkUpdateId { get; set; }

    [FieldOrder(5)]
    public sbyte Power { get; set; }

    [FieldOrder(6)]
    public sbyte ParentPriority { get; set; }

    [FieldOrder(7)]
    public ushort PanId { get; set; }

    [FieldOrder(8)]
    public EzspExtendedPanId ExtendedPanId { get; set; }

    [FieldOrder(9)]
    public ushort Sender { get; set; }

    [FieldOrder(10)]
    public bool Enhanced { get; set; }

    [FieldOrder(11)]
    public bool PermitJoin { get; set; }

    [FieldOrder(12)]
    public bool HasCapacity { get; set; }
}