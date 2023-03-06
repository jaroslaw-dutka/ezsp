using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspNetworkInitResponse
{
    [FieldOrder(0)]
    public EmberStatus Status { get; set; }
}