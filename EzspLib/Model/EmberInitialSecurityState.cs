using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmberInitialSecurityState
{
    public EmberInitialSecurityBitmask bitmask;
    public EmberKeyData preconfiguredKey;
    public EmberKeyData networkKey;
    public byte networkKeySequenceNumber;
    public ulong preconfiguredTrustCenterEui64;
}