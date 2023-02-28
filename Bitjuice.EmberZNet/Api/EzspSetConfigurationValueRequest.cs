using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspSetConfigurationValueRequest
{
    [FieldOrder(0)]
    public EzspConfigId ConfigId { get; set; }

    [FieldOrder(1)]
    public ushort Value { get; set; }
}