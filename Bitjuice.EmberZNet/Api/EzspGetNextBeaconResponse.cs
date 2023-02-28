using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspGetNextBeaconResponse
{
    [FieldOrder(0)]
    public EmberStatus Status { get; set; }

    [FieldOrder(1)]
    public EmberBeaconData Beacon { get; set; }
}