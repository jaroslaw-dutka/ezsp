public class EzspFrame
{
    public byte Sequence { get; set; }
    public ushort Control { get; set; }
    public ushort Id { get; set; }
    public byte[] Parameters { get; set; }
}