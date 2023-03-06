using BinarySerialization;

namespace Bitjuice.EmberZNet.Api;

public class EzspAddEndpointRequest
{
    [FieldOrder(0)]
    public byte Endpoint { get; set; }

    [FieldOrder(1)]
    public ZigBeeProfileId ProfileId { get; set; }

    [FieldOrder(2)]
    public ZigBeeDeviceId DeviceId { get; set; }

    [FieldOrder(3)]
    public byte AppFlags { get; set; }

    [FieldOrder(4)]
    public byte InputClusterCount { get; set; }

    [FieldOrder(5)]
    public byte OutputClusterCount { get; set; }

    [FieldOrder(6)]
    [FieldCount(nameof(InputClusterCount))]
    public ushort[] InputClusterList { get; set; }

    [FieldOrder(7)]
    [FieldCount(nameof(OutputClusterCount))]
    public ushort[] OutputClusterList { get; set; }
}