using BinarySerialization;
using EzspLib.Model;

namespace EzspLib.Requests;

public class EzspGetFirstBeaconResponse
{
    [FieldOrder(0)]
    public EmberStatus Status { get; set; }

    [FieldOrder(1)]
    public EmberBeaconIterator Iterator { get; set; }
}