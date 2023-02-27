using System.Runtime.InteropServices;
using Ezsp.Ember;

namespace Ezsp;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspJoinNetworkRequest
{
    public EmberNodeType nodeType;
    public EmberNetworkParameters parameters;
}