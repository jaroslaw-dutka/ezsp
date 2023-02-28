using BinarySerialization;

namespace EzspLib.Model;

public class EmberZigbeeNetwork
{
    [FieldOrder(0)]
    public byte Channel { get; set; }

    [FieldOrder(1)]
    public ushort PanId { get; set; }

    [FieldOrder(2)]
    public EzspExtendedPanId ExtendedPanId { get; set; }

    [FieldOrder(3)]
    public bool AllowingJoin { get; set; }

    [FieldOrder(4)]
    public byte StackProfile { get; set; }

    [FieldOrder(5)]
    public byte NwkUpdateId { get; set; }
}