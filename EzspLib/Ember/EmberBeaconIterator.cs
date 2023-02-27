using System.Runtime.InteropServices;

namespace EzspLib.Ember;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmberBeaconIterator
{
    public EmberBeaconData beacon;
    public byte index;
}