using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspSetConfigurationValueRequest
{
    public EzspConfigId configId;
    public ushort value;
}