using System.Text;
using BinarySerialization;

namespace Bitjuice.EmberZNet.Model;

public class EmberKeyData
{
    [FieldOrder(0)]
    [FieldLength(16)]
    public byte[] Data { get; set; }

    public EmberKeyData()
    {
    }

    public EmberKeyData(IEnumerable<byte> data)
    {
        Data = data.ToArray();
    }

    public EmberKeyData(byte data): this(Enumerable.Repeat(data, 16))
    {
    }

    public EmberKeyData(string key): this(Encoding.ASCII.GetBytes(key))
    {
    }
}