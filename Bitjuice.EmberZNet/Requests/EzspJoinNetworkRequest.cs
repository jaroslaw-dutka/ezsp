using BinarySerialization;
using EzspLib.Model;

namespace EzspLib.Requests;

public class EzspJoinNetworkRequest
{
    [FieldOrder(0)]
    public EmberNodeType NodeType { get; set; }

    [FieldOrder(1)]
    public EmberNetworkParameters Parameters { get; set; }
}