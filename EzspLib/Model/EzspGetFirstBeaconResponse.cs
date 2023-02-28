using BinarySerialization;

namespace EzspLib.Model;

public class EzspGetFirstBeaconResponse
{
    [FieldOrder(0)]
    public EmberStatus Status { get; set; }

    [FieldOrder(1)]
    public EmberBeaconIterator Iterator { get; set; }
}