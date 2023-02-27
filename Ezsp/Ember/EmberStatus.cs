﻿namespace Ezsp.Ember;

public enum EmberStatus: byte
{
    EMBER_SUCCESS = 0x00,
    EMBER_ERR_FATAL = 0x01,
    EMBER_BAD_ARGUMENT = 0x02,
    EMBER_EEPROM_MFG_STACK_VERSION_MISMATCH = 0x04,
    EMBER_INCOMPATIBLE_STATIC_MEMORY_DEFINITIONS = 0x05,
    EMBER_EEPROM_MFG_VERSION_MISMATCH = 0x06,
    EMBER_EEPROM_STACK_VERSION_MISMATCH = 0x07,
    EMBER_NO_BUFFERS = 0x18,
    EMBER_SERIAL_INVALID_BAUD_RATE = 0x20,
    EMBER_SERIAL_INVALID_PORT = 0x21,
    EMBER_SERIAL_TX_OVERFLOW = 0x22,
    EMBER_SERIAL_RX_OVERFLOW = 0x23,
    EMBER_SERIAL_RX_FRAME_ERROR = 0x24,
    EMBER_SERIAL_RX_PARITY_ERROR = 0x25,
    EMBER_SERIAL_RX_EMPTY = 0x26,
    EMBER_SERIAL_RX_OVERRUN_ERROR = 0x27,
    EMBER_MAC_TRANSMIT_QUEUE_FULL = 0x39,
    EMBER_MAC_UNKNOWN_HEADER_TYPE = 0x3A,
    EMBER_MAC_SCANNING = 0x3D,
    EMBER_MAC_NO_DATA = 0x31,
    EMBER_MAC_JOINED_NETWORK = 0x32,
    EMBER_MAC_BAD_SCAN_DURATION = 0x33,
    EMBER_MAC_INCORRECT_SCAN_TYPE = 0x34,
    EMBER_MAC_INVALID_CHANNEL_MASK = 0x35,
    EMBER_MAC_COMMAND_TRANSMIT_FAILURE = 0x36,
    EMBER_MAC_NO_ACK_RECEIVED = 0x40,
    EMBER_MAC_INDIRECT_TIMEOUT = 0x42,
    EMBER_SIM_EEPROM_ERASE_PAGE_GREEN = 0x43,
    EMBER_SIM_EEPROM_ERASE_PAGE_RED = 0x44,
    EMBER_SIM_EEPROM_FULL = 0x45,
    EMBER_ERR_FLASH_WRITE_INHIBITED = 0x46,
    EMBER_ERR_FLASH_VERIFY_FAILED = 0x47,
    EMBER_SIM_EEPROM_INIT_1_FAILED = 0x48,
    EMBER_SIM_EEPROM_INIT_2_FAILED = 0x49,
    EMBER_SIM_EEPROM_INIT_3_FAILED = 0x4A,
    EMBER_ERR_FLASH_PROG_FAIL = 0x4B,
    EMBER_ERR_FLASH_ERASE_FAIL = 0x4C,
    EMBER_ERR_BOOTLOADER_TRAP_TABLE_BAD = 0x58,
    EMBER_ERR_BOOTLOADER_TRAP_UNKNOWN = 0x59,
    EMBER_ERR_BOOTLOADER_NO_IMAGE = 0x5A,
    EMBER_DELIVERY_FAILED = 0x66,
    EMBER_BINDING_INDEX_OUT_OF_RANGE = 0x69,
    EMBER_ADDRESS_TABLE_INDEX_OUT_OF_RANGE = 0x6A,
    EMBER_INVALID_BINDING_INDEX = 0x6C,
    EMBER_INVALID_CALL = 0x70,
    EMBER_COST_NOT_KNOWN = 0x71,
    EMBER_MAX_MESSAGE_LIMIT_REACHED = 0x72,
    EMBER_MESSAGE_TOO_LONG = 0x74,
    EMBER_BINDING_IS_ACTIVE = 0x75,
    EMBER_ADDRESS_TABLE_ENTRY_IS_ACTIVE = 0x76,
    EMBER_ADC_CONVERSION_DONE = 0x80,
    EMBER_ADC_CONVERSION_BUSY = 0x81,
    EMBER_ADC_CONVERSION_DEFERRED = 0x82,
    EMBER_ADC_NO_CONVERSION_PENDING = 0x84,
    EMBER_SLEEP_INTERRUPTED = 0x85,
    EMBER_PHY_TX_UNDERFLOW = 0x88,
    EMBER_PHY_TX_INCOMPLETE = 0x89,
    EMBER_PHY_INVALID_CHANNEL = 0x8A,
    EMBER_PHY_INVALID_POWER = 0x8B,
    EMBER_PHY_TX_BUSY = 0x8C,
    EMBER_PHY_TX_CCA_FAIL = 0x8D,
    EMBER_PHY_OSCILLATOR_CHECK_FAILED = 0x8E,
    EMBER_PHY_ACK_RECEIVED = 0x8F,
    EMBER_NETWORK_UP = 0x90,
    EMBER_NETWORK_DOWN = 0x91,
    EMBER_JOIN_FAILED = 0x94,
    EMBER_MOVE_FAILED = 0x96,
    EMBER_CANNOT_JOIN_AS_ROUTER = 0x98,
    EMBER_NODE_ID_CHANGED = 0x99,
    EMBER_PAN_ID_CHANGED = 0x9A,
    EMBER_NO_BEACONS = 0xAB,
    EMBER_RECEIVED_KEY_IN_THE_CLEAR = 0xAC,
    EMBER_NO_NETWORK_KEY_RECEIVED = 0xAD,
    EMBER_NO_LINK_KEY_RECEIVED = 0xAE,
    EMBER_PRECONFIGURED_KEY_REQUIRED = 0xAF,
    EMBER_NOT_JOINED = 0x93,
    EMBER_INVALID_SECURITY_LEVEL = 0x95,
    EMBER_NETWORK_BUSY = 0xA1,
    EMBER_INVALID_ENDPOINT = 0xA3,
    EMBER_BINDING_HAS_CHANGED = 0xA4,
    EMBER_INSUFFICIENT_RANDOM_DATA = 0xA5,
    EMBER_APS_ENCRYPTION_ERROR = 0xA6,
    EMBER_SECURITY_STATE_NOT_SET = 0xA8,
    EMBER_KEY_TABLE_INVALID_ADDRESS = 0xB3,
    EMBER_SECURITY_CONFIGURATION_INVALID = 0xB7,
    EMBER_TOO_SOON_FOR_SWITCH_KEY = 0xB8,
    EMBER_KEY_NOT_AUTHORIZED = 0xBB,
    EMBER_SECURITY_DATA_INVALID = 0xBD,
    EMBER_SOURCE_ROUTE_FAILURE = 0xA9,
    EMBER_MANY_TO_ONE_ROUTE_FAILURE = 0xAA,
    EMBER_STACK_AND_HARDWARE_MISMATCH = 0xB0,
    EMBER_INDEX_OUT_OF_RANGE = 0xB1,
    EMBER_TABLE_FULL = 0xB4,
    EMBER_TABLE_ENTRY_ERASED = 0xB6,
    EMBER_LIBRARY_NOT_PRESENT = 0xB5,
    EMBER_OPERATION_IN_PROGRESS = 0xBA,
    EMBER_APPLICATION_ERROR_0 = 0xF0,
    EMBER_APPLICATION_ERROR_1 = 0xF1,
    EMBER_APPLICATION_ERROR_2 = 0xF2,
    EMBER_APPLICATION_ERROR_3 = 0xF3,
    EMBER_APPLICATION_ERROR_4 = 0xF4,
    EMBER_APPLICATION_ERROR_5 = 0xF5,
    EMBER_APPLICATION_ERROR_6 = 0xF6,
    EMBER_APPLICATION_ERROR_7 = 0xF7,
    EMBER_APPLICATION_ERROR_8 = 0xF8,
    EMBER_APPLICATION_ERROR_9 = 0xF9,
    EMBER_APPLICATION_ERROR_10 = 0xFA,
    EMBER_APPLICATION_ERROR_11 = 0xFB,
    EMBER_APPLICATION_ERROR_12 = 0xFC,
    EMBER_APPLICATION_ERROR_13 = 0xFD,
    EMBER_APPLICATION_ERROR_14 = 0xFE,
    EMBER_APPLICATION_ERROR_15 = 0xFF,
}