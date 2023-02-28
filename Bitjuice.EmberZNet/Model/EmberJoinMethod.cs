namespace Bitjuice.EmberZNet.Model;

public enum EmberJoinMethod : byte
{
    USE_MAC_ASSOCIATION = 0x0,
    USE_NWK_REJOIN = 0x1,
    USE_NWK_REJOIN_HAVE_NWK_KEY = 0x2,
    USE_CONFIGURED_NWK_STATE = 0x3
}