using BinarySerialization;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Api;

public class EzspSetInitialSecurityStateRequest
{
    [FieldOrder(0)]
    public EmberInitialSecurityState State { get; set; }
}