using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmberZigbeeNetwork
{
    public byte channel;
    public ushort panId;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] extendedPanId;
    public bool allowingJoin;
    public byte stackProfile;
    public byte nwkUpdateId;
}