using BinarySerialization;

namespace Bitjuice.EmberZNet.Api;

public class EzspGetMfgTokenResponse
{
    [FieldOrder(0)]
    public byte Length { get; set; }

    [FieldOrder(1)]
    [FieldLength(nameof(Length))]
    public string Data { get; set; }
}