using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace EzspLib.Ezsp;

public class EzspSerializer
{
    public static byte[] Serialize<T>(T obj) where T : struct
    {
        var bytes = new byte[Marshal.SizeOf<T>()];
        SerializeInternal(obj, bytes);
        return bytes;
    }

    public static T Deserialize<T>(Span<byte> bytes) where T : struct
    {
        return (T)DeserializeInternal(typeof(T), bytes);
    }

    private static void SerializeInternal(object obj, Span<byte> buffer)
    {
        foreach (var info in obj.GetType().GetFields())
        {
            var fieldType = info.FieldType;
            if (fieldType.IsEnum)
                fieldType = fieldType.GetEnumUnderlyingType();
            var value = info.GetValue(obj);

            if (fieldType == typeof(bool))
                buffer[0] = (byte)value;
            else if (fieldType == typeof(byte))
                buffer[0] = (byte)value;
            else if (fieldType == typeof(sbyte))
                buffer[0] = (byte)value;
            else if (fieldType == typeof(short))
                BinaryPrimitives.WriteInt16LittleEndian(buffer, (short)value);
            else if (fieldType == typeof(ushort))
                BinaryPrimitives.WriteUInt16LittleEndian(buffer, (ushort)value);
            else if (fieldType == typeof(int))
                BinaryPrimitives.WriteInt32LittleEndian(buffer, (int)value);
            else if (fieldType == typeof(uint))
                BinaryPrimitives.WriteUInt32LittleEndian(buffer, (uint)value);
            else if (fieldType == typeof(long))
                BinaryPrimitives.WriteInt64LittleEndian(buffer, (long)value);
            else if (fieldType == typeof(ulong))
                BinaryPrimitives.WriteUInt64LittleEndian(buffer, (ulong)value);
            else if (fieldType is { IsAnsiClass: true, IsValueType: true })
                SerializeInternal(value, buffer);
            else
                throw new InvalidOperationException($"Not supported type: {fieldType.Name}");

            buffer = buffer.Slice(Marshal.SizeOf(fieldType));
        }
    }

    public static object DeserializeInternal(Type type, Span<byte> buffer)
    {
        var obj = Activator.CreateInstance(type);
        foreach (var info in obj.GetType().GetFields())
        {
            var fieldType = info.FieldType;
            if (fieldType.IsEnum)
                fieldType = fieldType.GetEnumUnderlyingType();

            if (fieldType == typeof(bool))
                info.SetValue(obj, buffer[0] > 0);
            else if (fieldType == typeof(byte))
                info.SetValue(obj, buffer[0]);
            else if (fieldType == typeof(sbyte))
                info.SetValue(obj, buffer[0]);
            else if (fieldType == typeof(short))
                info.SetValue(obj, BinaryPrimitives.ReadInt16LittleEndian(buffer));
            else if (fieldType == typeof(ushort))
                info.SetValue(obj, BinaryPrimitives.ReadUInt16LittleEndian(buffer));
            else if (fieldType == typeof(int))
                info.SetValue(obj, BinaryPrimitives.ReadInt32LittleEndian(buffer));
            else if (fieldType == typeof(uint))
                info.SetValue(obj, BinaryPrimitives.ReadUInt32LittleEndian(buffer));
            else if (fieldType == typeof(long))
                info.SetValue(obj, BinaryPrimitives.ReadInt64LittleEndian(buffer));
            else if (fieldType == typeof(ulong))
                info.SetValue(obj, BinaryPrimitives.ReadUInt64LittleEndian(buffer));
            else if (fieldType is { IsAnsiClass: true, IsValueType: true })
                info.SetValue(obj, DeserializeInternal(fieldType, buffer));
            else
                throw new InvalidOperationException($"Not supported type: {fieldType.Name}");

            buffer = buffer.Slice(Marshal.SizeOf(fieldType));
        }
        return obj;
    }
}