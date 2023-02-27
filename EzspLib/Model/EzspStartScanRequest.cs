using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspStartScanRequest
{
    public EzspNetworkScanType scanType;
    public uint channelMask;
    public byte duration;
}