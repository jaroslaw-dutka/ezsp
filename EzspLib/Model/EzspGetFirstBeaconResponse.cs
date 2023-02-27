﻿using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspGetFirstBeaconResponse
{
    public EmberStatus status;
    public EmberBeaconIterator iterator;
}