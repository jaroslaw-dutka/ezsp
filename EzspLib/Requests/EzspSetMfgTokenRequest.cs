using BinarySerialization;
using EzspLib.Model;

namespace EzspLib.Requests;

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

public class EzspGetMfgTokenRequest
{
    [FieldOrder(0)]
    public EzspMfgTokenId TokenId { get; set; }
}

public class EzspGetMfgTokenResponse
{
    [FieldOrder(0)]
    public byte Length { get; set; }

    [FieldOrder(1)]
    [FieldLength(nameof(Length))]
    public string Data { get; set; }
}