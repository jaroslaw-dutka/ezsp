using BinarySerialization;

namespace EzspLib.Requests;

public class EzspVersionResponse
{
    [FieldOrder(0)]
    public byte EzspVersion { get; set; }

    [FieldOrder(1)]
    public byte StackType { get; set; }

    [FieldOrder(2)]
    public ushort StackVersion { get; set; }
}