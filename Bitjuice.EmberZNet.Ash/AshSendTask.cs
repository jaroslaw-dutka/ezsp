namespace Bitjuice.EmberZNet.Ash;

internal class AshSendTask
{
    public byte[] Data { get; }
    public bool Rejected { get; private set; }
    public int Retries { get; private set; }
    public DateTime SendTime { get; private set; }

    public AshSendTask(byte[] data)
    {
        Data = data;
    }

    public void MarkAsSent()
    {
        Rejected = false;
        Retries++;
        SendTime = DateTime.UtcNow;
    }

    public void MarkAsRejected()
    {
        Rejected = true;
    }

    public bool ShouldBeResend()
    {
        return Rejected || DateTime.UtcNow - SendTime > TimeSpan.FromMilliseconds(100);
    }
}