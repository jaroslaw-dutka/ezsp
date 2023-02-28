using BinarySerialization;
using EzspLib.Model;

namespace EzspLib.Requests;

public class EzspStartScanRequest
{
    [FieldOrder(0)]
    public EzspNetworkScanType ScanType { get; set; }

    [FieldOrder(1)]
    public uint ChannelMask { get; set; }

    [FieldOrder(2)]
    public byte Duration { get; set; }
}