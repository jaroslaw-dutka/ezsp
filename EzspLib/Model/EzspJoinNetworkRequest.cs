using BinarySerialization;

namespace EzspLib.Model;

public class EzspJoinNetworkRequest
{
    [FieldOrder(0)]
    public EmberNodeType NodeType { get; set; }

    [FieldOrder(1)]
    public EmberNetworkParameters Parameters { get; set; }
}