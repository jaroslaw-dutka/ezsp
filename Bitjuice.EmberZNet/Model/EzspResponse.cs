using BinarySerialization;

namespace Bitjuice.EmberZNet.Model;

public class EzspResponse
{
    [FieldOrder(0)]
    public EzspStatus Status { get; set; }
}