using BinarySerialization;

namespace EzspLib.Model;

public class EmberBeaconIterator
{
    [FieldOrder(0)]
    public EmberBeaconData Beacon { get; set; }

    [FieldOrder(1)]
    public byte Index { get; set; }
}