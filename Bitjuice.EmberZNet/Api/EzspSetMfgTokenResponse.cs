using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspSetMfgTokenResponse
{
    [FieldOrder(0)]
    public EmberStatus Status { get; set; }
}