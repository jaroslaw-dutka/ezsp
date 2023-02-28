using BinarySerialization;
using EzspLib.Model;

namespace EzspLib.Requests;

public class EzspNetworkStateResponse
{
    [FieldOrder(0)]
    public EmberNetworkStatus Status { get; set; }
}