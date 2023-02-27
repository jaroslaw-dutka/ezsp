using System.Runtime.InteropServices;

namespace Ezsp.Ember;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmberNodeId
{
    public ushort address;
}