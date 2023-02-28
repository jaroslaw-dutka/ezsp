namespace EzspLib.Model;

public enum EmberNetworkStatus
{
    NO_NETWORK = 0x00,
    JOINING_NETWORK = 0x01,
    JOINED_NETWORK = 0x02,
    JOINED_NETWORK_NO_PARENT = 0x03,
    LEAVING_NETWORK = 0x04
}