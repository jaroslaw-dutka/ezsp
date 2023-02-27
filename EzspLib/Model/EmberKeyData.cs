using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmberKeyData
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
    public byte[] data;
}