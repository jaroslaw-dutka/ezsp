using BinarySerialization;

namespace Bitjuice.EmberZNet;

public class EzspSerializer
{
    private static readonly BinarySerializer Serializer = new();

    public static byte[] Serialize<T>(T obj)
    {
        var stream = new MemoryStream();
        Serializer.Serialize(stream, obj);
        return stream.ToArray();
    }

    public static T Deserialize<T>(ReadOnlyMemory<byte> bytes)
    {
        var stream = new ReadOnlyMemoryStream(bytes);
        return Serializer.Deserialize<T>(stream);
    }
}