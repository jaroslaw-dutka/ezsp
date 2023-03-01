namespace Bitjuice.EmberZNet.Model;

public enum EmberIncomingMessageType: byte
{
    IncomingUnicast = 0x00,
    IncomingUnicastReply = 0x01,
    IncomingMulticast = 0x02,
    IncomingMulticastLoopback = 0x03,
    IncomingBroadcast = 0x04,
    IncomingBroadcastLoopback = 0x05,
    IncomingManyToOneRouteRequest = 0x06,
}