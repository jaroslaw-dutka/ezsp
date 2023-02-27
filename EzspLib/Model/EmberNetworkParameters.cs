using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmberNetworkParameters
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] extendedPanId;
    public ushort panId;
    public byte radioTxPower;
    public byte radioChannel;
    public EmberJoinMethod joinMethod;
    public ushort nwkManagerId;
    public byte nwkUpdateId;
    public uint channels;
}