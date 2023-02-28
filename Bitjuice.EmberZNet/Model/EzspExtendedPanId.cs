using BinarySerialization;

namespace Bitjuice.EmberZNet.Model;

public class EzspExtendedPanId
{
    [FieldOrder(0)]
    [FieldLength(8)]
    public byte[] ExtendedPanId { get; set; }

    public EzspExtendedPanId()
    {
    }

    public EzspExtendedPanId(ulong epid)
    {
        ExtendedPanId = BitConverter.GetBytes(epid);
    }
}