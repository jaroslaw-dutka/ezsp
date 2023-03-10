using BinarySerialization;

namespace Bitjuice.EmberZNet.Model;

public class EmberInitialSecurityState
{
    [FieldOrder(0)]
    public EmberInitialSecurity Bitmask { get; set; }

    [FieldOrder(1)]
    public EmberKeyData PreconfiguredKey { get; set; }

    [FieldOrder(2)]
    public EmberKeyData NetworkKey { get; set; }

    [FieldOrder(3)]
    public byte NetworkKeySequenceNumber { get; set; }

    [FieldOrder(4)]
    public ulong PreconfiguredTrustCenterEui64 { get; set; }
}