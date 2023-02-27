using System.Runtime.InteropServices;

namespace Ezsp.Ember;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmberKeyData
{
    public ulong field1;
    public ulong field2;
}