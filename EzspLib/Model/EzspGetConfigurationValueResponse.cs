using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspGetConfigurationValueResponse
{
    public EzspStatus status;
    public ushort value;
}