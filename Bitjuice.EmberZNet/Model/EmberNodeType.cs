namespace Bitjuice.EmberZNet.Model;

public enum EmberNodeType : byte
{
    UnknownDevice = 0x00, // Device is not joined.
    Coordinator = 0x01, // Will relay messages and can act as a parent to other nodes.
    Router = 0x02, // Will relay messages and can act as a parent to other nodes.
    EndDevice = 0x03, // Communicates only with its parent and will not relay messages.
    SleepyEndDevice = 0x04, // An end device whose radio can be turned off to save power. The application must poll to receive messages.
}