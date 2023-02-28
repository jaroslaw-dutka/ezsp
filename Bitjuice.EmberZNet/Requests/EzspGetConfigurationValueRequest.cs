using BinarySerialization;
using EzspLib.Model;

namespace EzspLib.Requests;

public class EzspGetConfigurationValueRequest
{
    [FieldOrder(0)]
    public EzspConfigId ConfigId { get; set; }
}