using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspGetConfigurationValueResponse
{
    [FieldOrder(0)]
    public EzspStatus Status { get; set; }

    [FieldOrder(1)]
    public ushort Value { get; set; }
}