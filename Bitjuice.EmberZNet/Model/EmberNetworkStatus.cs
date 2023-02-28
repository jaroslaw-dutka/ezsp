namespace Bitjuice.EmberZNet.Model;

public enum EmberNetworkStatus
{
    NoNetwork = 0x00,
    JoiningNetwork = 0x01,
    JoinedNetwork = 0x02,
    JoinedNetworkNoParent = 0x03,
    LeavingNetwork = 0x04
}