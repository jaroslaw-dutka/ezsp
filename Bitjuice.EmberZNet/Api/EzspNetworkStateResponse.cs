using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspNetworkStateResponse
{
    [FieldOrder(0)]
    public EmberNetworkStatus Status { get; set; }
}