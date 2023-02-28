using BinarySerialization;

namespace EzspLib.Requests;

public class EzspVersionRequest
{
    [FieldOrder(0)]
    public byte EzspVersion { get; set; }
}