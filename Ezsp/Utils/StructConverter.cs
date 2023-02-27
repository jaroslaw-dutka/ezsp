using System.Runtime.InteropServices;

namespace Ezsp.Utils
{
    public class StructConverter
    {
        public static byte[] StructToBytes<T>(T obj) where T : struct
        {
            var size = Marshal.SizeOf<T>();
            var bytes = new byte[size];
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(obj, ptr, true);
            Marshal.Copy(ptr, bytes, 0, size);
            Marshal.FreeHGlobal(ptr);
            return bytes;
        }

        public static T BytesToStruct<T>(byte[] bytes) where T: struct
        {
            var size = Marshal.SizeOf<T>();
            var ptr = Marshal.AllocHGlobal(size);
            Marshal.Copy(bytes, 0, ptr, size);
            var obj = Marshal.PtrToStructure<T>(ptr);
            Marshal.FreeHGlobal(ptr);
            return obj;
        }
    }
}
