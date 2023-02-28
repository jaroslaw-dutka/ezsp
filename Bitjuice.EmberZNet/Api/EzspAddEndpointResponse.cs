using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspAddEndpointResponse
{
    [FieldOrder(0)]
    public EzspStatus Status { get; set; }
}