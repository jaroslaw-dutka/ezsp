﻿namespace Ezsp.Ember;

[Flags]
public enum EmberInitialSecurityBitmask: ushort
{
    EMBER_STANDARD_SECURITY_MODE = 0x0000,
    EMBER_DISTRIBUTED_TRUST_CENTER_MODE = 0x0002,
    EMBER_TRUST_CENTER_GLOBAL_LINK_KEY = 0x0004,
    EMBER_PRECONFIGURED_NETWORK_KEY_MODE = 0x0008,
    EMBER_TRUST_CENTER_USES_HASHED_LINK_KEY = 0x0084,
    EMBER_HAVE_PRECONFIGURED_KEY = 0x0100,
    EMBER_HAVE_NETWORK_KEY = 0x0200,
    EMBER_GET_LINK_KEY_WHEN_JOINING = 0x0400,
    EMBER_REQUIRE_ENCRYPTED_KEY = 0x0800,
    EMBER_NO_FRAME_COUNTER_RESET = 0x1000,
    EMBER_GET_PRECONFIGURED_KEY_FROM_INSTALL_CODE = 0x2000,
    EMBER_HAVE_TRUST_CENTER_EUI64 = 0x0040,
}