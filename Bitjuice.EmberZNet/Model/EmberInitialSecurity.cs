namespace Bitjuice.EmberZNet.Model;

[Flags]
public enum EmberInitialSecurity : ushort
{
    StandardSecurityMode = 0x0000,
    DistributedTrustCenterMode = 0x0002,
    TrustCenterGlobalLinkKey = 0x0004,
    PreconfiguredNetworkKeyMode = 0x0008,
    TrustCenterUsesHashedLinkKey = 0x0084,
    HavePreconfiguredKey = 0x0100,
    HaveNetworkKey = 0x0200,
    GetLinkKeyWhenJoining = 0x0400,
    RequireEncryptedKey = 0x0800,
    NoFrameCounterReset = 0x1000,
    GetPreconfiguredKeyFromInstallCode = 0x2000,
    HaveTrustCenterEui64 = 0x0040,
}