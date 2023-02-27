using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspGetNextBeaconResponse
{
    public EmberStatus status;
    public EmberBeaconData beacon;
}