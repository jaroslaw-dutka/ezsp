namespace Bitjuice.EmberZNet.Ash;

internal class AshDataSendTask
{
    public byte[] Data { get; set; }
    public int Retries { get; set; }
    public DateTime SendTime { get; set; }
}