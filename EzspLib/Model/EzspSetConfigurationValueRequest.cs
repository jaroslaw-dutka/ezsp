using BinarySerialization;

namespace EzspLib.Model;

public class EzspSetConfigurationValueRequest
{
    [FieldOrder(0)]
    public EzspConfigId ConfigId { get; set; }

    [FieldOrder(1)]
    public ushort Value { get; set; }
}