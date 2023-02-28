namespace EzspLib.Model;

public enum EmberNodeType : byte
{
    UNKNOWN_DEVICE = 0x00, // Device is not joined.
    COORDINATOR = 0x01, // Will relay messages and can act as a parent to other nodes.
    ROUTER = 0x02, // Will relay messages and can act as a parent to other nodes.
    END_DEVICE = 0x03, // Communicates only with its parent and will not relay messages.
    SLEEPY_END_DEVICE = 0x04, // An end device whose radio can be turned off to save power. The application must poll to receive messages.
}