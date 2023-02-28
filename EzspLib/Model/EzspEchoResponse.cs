using BinarySerialization;

namespace EzspLib.Model;

public class EzspEchoResponse
{
    [FieldOrder(0)]
    public byte Length { get; set; }

    [FieldOrder(1)]
    [FieldLength(nameof(Length))]
    public string Data { get; set; }
}