using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmberZigbeeNetwork
{
    public byte channel;
    public ushort panId;
    public ulong extendedPanId;
    public bool allowingJoin;
    public byte stackProfile;
    public byte nwkUpdateId;
}