using BinarySerialization;

namespace Bitjuice.EmberZNet.Api;

public class EzspVersionRequest
{
    [FieldOrder(0)]
    public byte EzspVersion { get; set; }
}