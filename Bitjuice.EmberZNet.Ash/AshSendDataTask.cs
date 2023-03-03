namespace Bitjuice.EmberZNet.Ash;

internal class AshSendDataTask
{
    public byte[] Data { get; }
    public bool NotAccepted { get; private set; }
    public int Retries { get; private set; }
    public DateTime SendTime { get; private set; }

    public AshSendDataTask(byte[] data)
    {
        Data = data;
    }

    public void MarkAsSent()
    {
        NotAccepted = false;
        Retries++;
        SendTime = DateTime.UtcNow;
    }

    public void MarkAsNotAccepted()
    {
        NotAccepted = true;
    }

    public bool ShouldBeResend()
    {
        return NotAccepted || DateTime.UtcNow - SendTime > TimeSpan.FromMilliseconds(100);
    }
}