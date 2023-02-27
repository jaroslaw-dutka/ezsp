﻿using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EmberBeaconData
{
    public byte channel;
    public byte lqi;
    public sbyte rssi;
    public byte depth;
    public byte nwkUpdateId;
    public sbyte power;
    public sbyte parentPriority;
    public ushort panId;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] extendedPanId;
    public ushort sender;
    public bool enhanced;
    public bool permitJoin;
    public bool hasCapacity;
}