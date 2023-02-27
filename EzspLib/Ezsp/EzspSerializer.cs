using System.Runtime.InteropServices;

namespace EzspLib.Ezsp;

public class EzspSerializer
{
    public static byte[] Serialize<T>(T obj) where T : struct
    {
        var size = Marshal.SizeOf(obj);
        var bytes = new byte[size];
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.StructureToPtr(obj, ptr, true);
        Marshal.Copy(ptr, bytes, 0, size);
        Marshal.FreeHGlobal(ptr);
        return bytes;
    }

    public static T Deserialize<T>(byte[] bytes) where T : struct
    {
        var size = Marshal.SizeOf<T>();
        var ptr = Marshal.AllocHGlobal(size);
        Marshal.Copy(bytes, 0, ptr, size);
        var obj = Marshal.PtrToStructure<T>(ptr);
        Marshal.FreeHGlobal(ptr);
        return obj;
    }
}