using BinarySerialization;
using EzspLib.Model;

namespace EzspLib.Requests;

public class EzspGetNextBeaconResponse
{
    [FieldOrder(0)]
    public EmberStatus Status { get; set; }

    [FieldOrder(1)]
    public EmberBeaconData Beacon { get; set; }
}