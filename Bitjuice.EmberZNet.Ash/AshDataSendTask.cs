﻿namespace Bitjuice.EmberZNet.Ash;

internal class AshDataSendTask
{
    public byte[] Data { get; set; }
    public TaskCompletionSource Tcs { get; set; }
}