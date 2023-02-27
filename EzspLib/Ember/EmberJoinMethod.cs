namespace EzspLib.Ember;

public enum EmberJoinMethod : byte
{
    EMBER_USE_MAC_ASSOCIATION = 0x0,
    EMBER_USE_NWK_REJOIN = 0x1,
    EMBER_USE_NWK_REJOIN_HAVE_NWK_KEY = 0x2,
    EMBER_USE_CONFIGURED_NWK_STATE = 0x3
}