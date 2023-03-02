namespace Bitjuice.EmberZNet.Model;

public enum EzspMfgTokenId: byte
{
    MfgCustomVersion = 0x00,
    MfgString = 0x01,
    MfgBoardName = 0x02,
    MfgManufId = 0x03,
    MfgPhyConfig = 0x04,
    MfgBootloadAesKey = 0x05,
    MfgAshConfig = 0x06,
    MfgEzspStorage = 0x07,
    StackCalData = 0x08,
    MfgCbkeData = 0x09,
    MfgInstallationCode = 0x0A,
    StackCalFilter = 0x0B,
    MfgCustomEui64 = 0x0C,
    MfgCtune = 0x0D
}