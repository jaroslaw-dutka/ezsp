using System.Runtime.InteropServices;

namespace Ezsp.Ember;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmberNetworkParameters
{
    public ulong extendedPanId;
    public ushort panId;
    public byte radioTxPower;
    public byte radioChannel;
    public EmberJoinMethod joinMethod;
    public EmberNodeId nwkManagerId;
    public byte nwkUpdateId;
    public uint channels;
}