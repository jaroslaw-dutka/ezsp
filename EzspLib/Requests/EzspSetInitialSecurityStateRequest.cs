using BinarySerialization;
using EzspLib.Model;

namespace EzspLib.Requests;

public class EzspSetInitialSecurityStateRequest
{
    [FieldOrder(0)]
    public EmberInitialSecurityState State { get; set; }
}