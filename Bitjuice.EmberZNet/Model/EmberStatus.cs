﻿namespace Bitjuice.EmberZNet.Model;

public enum EmberStatus : byte
{
    SUCCESS = 0x00,
    ERR_FATAL = 0x01,
    BAD_ARGUMENT = 0x02,
    EEPROM_MFG_STACK_VERSION_MISMATCH = 0x04,
    INCOMPATIBLE_STATIC_MEMORY_DEFINITIONS = 0x05,
    EEPROM_MFG_VERSION_MISMATCH = 0x06,
    EEPROM_STACK_VERSION_MISMATCH = 0x07,
    NO_BUFFERS = 0x18,
    SERIAL_INVALID_BAUD_RATE = 0x20,
    SERIAL_INVALID_PORT = 0x21,
    SERIAL_TX_OVERFLOW = 0x22,
    SERIAL_RX_OVERFLOW = 0x23,
    SERIAL_RX_FRAME_ERROR = 0x24,
    SERIAL_RX_PARITY_ERROR = 0x25,
    SERIAL_RX_EMPTY = 0x26,
    SERIAL_RX_OVERRUN_ERROR = 0x27,
    MAC_TRANSMIT_QUEUE_FULL = 0x39,
    MAC_UNKNOWN_HEADER_TYPE = 0x3A,
    MAC_SCANNING = 0x3D,
    MAC_NO_DATA = 0x31,
    MAC_JOINED_NETWORK = 0x32,
    MAC_BAD_SCAN_DURATION = 0x33,
    MAC_INCORRECT_SCAN_TYPE = 0x34,
    MAC_INVALID_CHANNEL_MASK = 0x35,
    MAC_COMMAND_TRANSMIT_FAILURE = 0x36,
    MAC_NO_ACK_RECEIVED = 0x40,
    MAC_INDIRECT_TIMEOUT = 0x42,
    SIM_EEPROM_ERASE_PAGE_GREEN = 0x43,
    SIM_EEPROM_ERASE_PAGE_RED = 0x44,
    SIM_EEPROM_FULL = 0x45,
    ERR_FLASH_WRITE_INHIBITED = 0x46,
    ERR_FLASH_VERIFY_FAILED = 0x47,
    SIM_EEPROM_INIT_1_FAILED = 0x48,
    SIM_EEPROM_INIT_2_FAILED = 0x49,
    SIM_EEPROM_INIT_3_FAILED = 0x4A,
    ERR_FLASH_PROG_FAIL = 0x4B,
    ERR_FLASH_ERASE_FAIL = 0x4C,
    ERR_BOOTLOADER_TRAP_TABLE_BAD = 0x58,
    ERR_BOOTLOADER_TRAP_UNKNOWN = 0x59,
    ERR_BOOTLOADER_NO_IMAGE = 0x5A,
    DELIVERY_FAILED = 0x66,
    BINDING_INDEX_OUT_OF_RANGE = 0x69,
    ADDRESS_TABLE_INDEX_OUT_OF_RANGE = 0x6A,
    INVALID_BINDING_INDEX = 0x6C,
    INVALID_CALL = 0x70,
    COST_NOT_KNOWN = 0x71,
    MAX_MESSAGE_LIMIT_REACHED = 0x72,
    MESSAGE_TOO_LONG = 0x74,
    BINDING_IS_ACTIVE = 0x75,
    ADDRESS_TABLE_ENTRY_IS_ACTIVE = 0x76,
    ADC_CONVERSION_DONE = 0x80,
    ADC_CONVERSION_BUSY = 0x81,
    ADC_CONVERSION_DEFERRED = 0x82,
    ADC_NO_CONVERSION_PENDING = 0x84,
    SLEEP_INTERRUPTED = 0x85,
    PHY_TX_UNDERFLOW = 0x88,
    PHY_TX_INCOMPLETE = 0x89,
    PHY_INVALID_CHANNEL = 0x8A,
    PHY_INVALID_POWER = 0x8B,
    PHY_TX_BUSY = 0x8C,
    PHY_TX_CCA_FAIL = 0x8D,
    PHY_OSCILLATOR_CHECK_FAILED = 0x8E,
    PHY_ACK_RECEIVED = 0x8F,
    NETWORK_UP = 0x90,
    NETWORK_DOWN = 0x91,
    JOIN_FAILED = 0x94,
    MOVE_FAILED = 0x96,
    CANNOT_JOIN_AS_ROUTER = 0x98,
    NODE_ID_CHANGED = 0x99,
    PAN_ID_CHANGED = 0x9A,
    NO_BEACONS = 0xAB,
    RECEIVED_KEY_IN_THE_CLEAR = 0xAC,
    NO_NETWORK_KEY_RECEIVED = 0xAD,
    NO_LINK_KEY_RECEIVED = 0xAE,
    PRECONFIGURED_KEY_REQUIRED = 0xAF,
    NOT_JOINED = 0x93,
    INVALID_SECURITY_LEVEL = 0x95,
    NETWORK_BUSY = 0xA1,
    INVALID_ENDPOINT = 0xA3,
    BINDING_HAS_CHANGED = 0xA4,
    INSUFFICIENT_RANDOM_DATA = 0xA5,
    APS_ENCRYPTION_ERROR = 0xA6,
    SECURITY_STATE_NOT_SET = 0xA8,
    KEY_TABLE_INVALID_ADDRESS = 0xB3,
    SECURITY_CONFIGURATION_INVALID = 0xB7,
    TOO_SOON_FOR_SWITCH_KEY = 0xB8,
    KEY_NOT_AUTHORIZED = 0xBB,
    SECURITY_DATA_INVALID = 0xBD,
    SOURCE_ROUTE_FAILURE = 0xA9,
    MANY_TO_ONE_ROUTE_FAILURE = 0xAA,
    STACK_AND_HARDWARE_MISMATCH = 0xB0,
    INDEX_OUT_OF_RANGE = 0xB1,
    TABLE_FULL = 0xB4,
    TABLE_ENTRY_ERASED = 0xB6,
    LIBRARY_NOT_PRESENT = 0xB5,
    OPERATION_IN_PROGRESS = 0xBA,
    APPLICATION_ERROR_0 = 0xF0,
    APPLICATION_ERROR_1 = 0xF1,
    APPLICATION_ERROR_2 = 0xF2,
    APPLICATION_ERROR_3 = 0xF3,
    APPLICATION_ERROR_4 = 0xF4,
    APPLICATION_ERROR_5 = 0xF5,
    APPLICATION_ERROR_6 = 0xF6,
    APPLICATION_ERROR_7 = 0xF7,
    APPLICATION_ERROR_8 = 0xF8,
    APPLICATION_ERROR_9 = 0xF9,
    APPLICATION_ERROR_10 = 0xFA,
    APPLICATION_ERROR_11 = 0xFB,
    APPLICATION_ERROR_12 = 0xFC,
    APPLICATION_ERROR_13 = 0xFD,
    APPLICATION_ERROR_14 = 0xFE,
    APPLICATION_ERROR_15 = 0xFF,
}