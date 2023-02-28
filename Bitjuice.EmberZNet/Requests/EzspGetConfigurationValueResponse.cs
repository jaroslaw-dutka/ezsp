using BinarySerialization;
using EzspLib.Model;

namespace EzspLib.Requests;

public class EzspGetConfigurationValueResponse
{
    [FieldOrder(0)]
    public EzspStatus Status { get; set; }

    [FieldOrder(1)]
    public ushort Value { get; set; }
}