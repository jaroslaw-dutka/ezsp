using System.Runtime.InteropServices;
using Ezsp.Ember;

namespace Ezsp;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspResponse
{
    public EmberStatus status;
}