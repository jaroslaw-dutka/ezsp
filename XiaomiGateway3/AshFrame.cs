using System.Text;

public class AshFrame {
    public AshControl Control { get; set; }
    public byte[] Data { get; set; }
    public byte[] Crc { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Control: {Control.ToString()}");
        sb.AppendLine($"Data: {BitConverter.ToString(Data).Replace("-", " ")}");
        sb.AppendLine($"CRC: {BitConverter.ToString(Crc).Replace("-", " ")}");

        return sb.ToString();
    }
}