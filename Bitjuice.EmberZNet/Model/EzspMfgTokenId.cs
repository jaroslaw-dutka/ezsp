﻿namespace Bitjuice.EmberZNet.Model;

public enum EzspMfgTokenId: byte
{
    MFG_CUSTOM_VERSION = 0x00,
    MFG_STRING = 0x01,
    MFG_BOARD_NAME = 0x02,
    MFG_MANUF_ID = 0x03,
    MFG_PHY_CONFIG = 0x04,
    MFG_BOOTLOAD_AES_KEY = 0x05,
    MFG_ASH_CONFIG = 0x06,
    MFG_EZSP_STORAGE = 0x07,
    STACK_CAL_DATA = 0x08,
    MFG_CBKE_DATA = 0x09,
    MFG_INSTALLATION_CODE = 0x0A,
    STACK_CAL_FILTER = 0x0B,
    MFG_CUSTOM_EUI_64 = 0x0C,
    MFG_CTUNE = 0x0D,
}