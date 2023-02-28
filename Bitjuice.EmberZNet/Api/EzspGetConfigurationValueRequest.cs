using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspGetConfigurationValueRequest
{
    [FieldOrder(0)]
    public EzspConfigId ConfigId { get; set; }
}