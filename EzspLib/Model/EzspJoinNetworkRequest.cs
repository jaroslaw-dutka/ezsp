﻿using System.Runtime.InteropServices;

namespace EzspLib.Model;

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct EzspJoinNetworkRequest
{
    public EmberNodeType nodeType;
    public EmberNetworkParameters parameters;
}

public struct EzspSetInitialSecurityStateRequest
{
    public EmberInitialSecurityState state;
}