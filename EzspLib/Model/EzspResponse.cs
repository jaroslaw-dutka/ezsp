using BinarySerialization;

namespace EzspLib.Model;

public class EzspResponse
{
    [FieldOrder(0)]
    public EzspStatus Status { get; set; }
}