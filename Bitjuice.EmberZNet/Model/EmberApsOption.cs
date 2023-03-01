namespace Bitjuice.EmberZNet.Model;

[Flags]
public enum EmberApsOption: ushort
{
    None = 0x0000,
    Encryption = 0x0020,
    Retry = 0x0040,
    EnableRouteDiscovery = 0x0100,
    ForceRouteDiscovery = 0x0200,
    SourceEui64 = 0x0400,
    DestinationEui64 = 0x0800,
    EnableAddressDiscovery = 0x1000,
    PollResponse = 0x2000,
    ZdoResponseRequired = 0x4000,
    Fragment = 0x8000
}