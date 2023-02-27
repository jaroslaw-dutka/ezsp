using System.Runtime.InteropServices;
using EzspLib.Ember;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspJoinNetworkRequest
{
    public EmberNodeType nodeType;
    public EmberNetworkParameters parameters;
}