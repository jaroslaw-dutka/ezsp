namespace Bitjuice.EmberZNet.Model;

public enum EmberJoinMethod : byte
{
    UseMacAssociation = 0x0,
    UseNwkRejoin = 0x1,
    UseNwkRejoinHaveNwkKey = 0x2,
    UseConfiguredNwkState = 0x3
}