using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspGetFirstBeaconResponse
{
    [FieldOrder(0)]
    public EmberStatus Status { get; set; }

    [FieldOrder(1)]
    public EmberBeaconIterator Iterator { get; set; }
}