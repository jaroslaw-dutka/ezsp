using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspGetMfgTokenRequest
{
    [FieldOrder(0)]
    public EzspMfgTokenId TokenId { get; set; }
}