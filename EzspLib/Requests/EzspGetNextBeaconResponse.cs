using System.Runtime.InteropServices;
using EzspLib.Model;

namespace EzspLib.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspGetNextBeaconResponse
{
    public EmberStatus status;
    public EmberBeaconData beacon;
}