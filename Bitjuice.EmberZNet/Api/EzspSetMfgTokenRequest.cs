using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspSetMfgTokenRequest
{
    [FieldOrder(0)]
    public EzspMfgTokenId TokenId { get; set; }

    [FieldOrder(1)]
    public byte Length { get; set; }

    [FieldOrder(2)]
    [FieldLength(nameof(Length))]
    public string Data { get; set; }
}