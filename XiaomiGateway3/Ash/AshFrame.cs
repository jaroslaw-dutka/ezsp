using System.Text;

namespace XiaomiGateway3.Ash;

public class AshFrame
{
    public AshControl Control { get; set; }
    public byte[] Data { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Ctrl: {Control.ToString()}");
        sb.AppendLine($"Data: {BitConverter.ToString(Data).Replace("-", " ")}");

        return sb.ToString();
    }
}