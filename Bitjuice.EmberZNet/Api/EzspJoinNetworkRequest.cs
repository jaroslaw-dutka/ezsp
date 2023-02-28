using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspJoinNetworkRequest
{
    [FieldOrder(0)]
    public EmberNodeType NodeType { get; set; }

    [FieldOrder(1)]
    public EmberNetworkParameters Parameters { get; set; }
}