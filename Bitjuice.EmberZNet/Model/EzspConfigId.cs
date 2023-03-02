﻿namespace Bitjuice.EmberZNet.Model;

public enum EzspConfigId: byte
{
    PacketBufferCount = 0x01,
    NeighborTableSize = 0x02,
    ApsUnicastMessageCount = 0x03,
    BindingTableSize = 0x04,
    AddressTableSize = 0x05,
    MulticastTableSize = 0x06,
    RouteTableSize = 0x07,
    DiscoveryTableSize = 0x08,
    StackProfile = 0x0C,
    SecurityLevel = 0x0D,
    MaxHops = 0x10,
    MaxEndDeviceChildren = 0x11,
    IndirectTransmissionTimeout = 0x12,
    EndDevicePollTimeout = 0x13,
    TxPowerMode = 0x17,
    DisableRelay = 0x18,
    TrustCenterAddressCacheSize = 0x19,
    SourceRouteTableSize = 0x1A,
    FragmentWindowSize = 0x1C,
    FragmentDelayMs = 0x1D,
    KeyTableSize = 0x1E,
    ApsAckTimeout = 0x1F,
    BeaconJitterDuration = 0x20,
    EndDeviceBindTimeout = 0x21,
    PanIdConflictReportThreshold = 0x22,
    RequestKeyTimeout = 0x24,
    CertificateTableSize = 0x29,
    ApplicationZdoFlags = 0x2A,
    BroadcastTableSize = 0x2B,
    MacFilterTableSize = 0x2C,
    SupportedNetworks = 0x2D,
    SendMulticastsToSleepyAddress = 0x2E,
    ZllGroupAddresses = 0x2F,
    ZllRssiThreshold = 0x30,
    MtorrFlowControl = 0x33,
    RetryQueueSize = 0x34,
    NewBroadcastEntryThreshold = 0x35,
    BroadcastMinAcksNeeded = 0x37,
    TcRejoinsUsingWellKnownKeyTimeoutS = 0x38,
    CtuneValue = 0x39,
    AssumeTcConcentratorType = 0x40,
    GpProxyTableSize = 0x41,
    GpSinkTableSize = 0x42,

    TxK = 0x83,
}