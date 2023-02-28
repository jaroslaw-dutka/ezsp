using BinarySerialization;

namespace EzspLib.Model;

public class EzspSetInitialSecurityStateRequest
{
    [FieldOrder(0)]
    public EmberInitialSecurityState State { get; set; }
}