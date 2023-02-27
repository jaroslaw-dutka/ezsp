using System.Text;

namespace EzspLib.Ash;

public class AshFrame
{
    public AshControlByte Control { get; set; }
    public byte[]? Data { get; set; }

    public override string ToString()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"Ctrl: {Control.ToString()}");
        if (Data is not null)
            sb.AppendLine($"Data: {BitConverter.ToString(Data).Replace("-", " ")}");
        else
            sb.AppendLine("Data: EMPTY");

        return sb.ToString();
    }
}