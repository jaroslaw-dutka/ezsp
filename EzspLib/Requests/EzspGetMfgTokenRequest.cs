using BinarySerialization;
using EzspLib.Model;

namespace EzspLib.Requests;

public class EzspGetMfgTokenRequest
{
    [FieldOrder(0)]
    public EzspMfgTokenId TokenId { get; set; }
}