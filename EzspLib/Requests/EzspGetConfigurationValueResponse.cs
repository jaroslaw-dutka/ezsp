using System.Runtime.InteropServices;
using EzspLib.Model;

namespace EzspLib.Requests;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspGetConfigurationValueResponse
{
    public EzspStatus status;
    public ushort value;
}