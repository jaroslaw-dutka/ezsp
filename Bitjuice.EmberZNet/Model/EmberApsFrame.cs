using BinarySerialization;

namespace Bitjuice.EmberZNet.Model;

public class EmberApsFrame
{
    [FieldOrder(0)]
    public ushort ProfileId { get; set; }

    [FieldOrder(1)]
    public ushort ClusterId { get; set; }

    [FieldOrder(2)]
    public byte SourceEndpoint { get; set; }

    [FieldOrder(3)]
    public byte DestinationEndpoint { get; set; }

    [FieldOrder(4)]
    public EmberApsOption Options { get; set; }

    [FieldOrder(5)]
    public ushort GroupId { get; set; }

    [FieldOrder(6)]
    public byte Sequence { get; set; }

    // [FieldOrder(7)]
    // public byte Radius { get; set; }
}