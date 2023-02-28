using EzspLib.Ezsp;
using EzspLib.Model;

namespace EzspLib;

public class EzspClient
{
    private EzspChannel Channel { get; }

    public EzspClient(Stream stream): this(new EzspChannel(stream))
    {
    }

    public EzspClient(EzspChannel channel)
    {
        Channel = channel;
    }

    // public async Task<EzspResponse> VersionAsync(byte version)
    // {
    //     return await Channel.SendAsync(EzspCommand.Version, 7);
    // }

    public async Task<string> EchoAsync(string text)
    {
        var response = await Channel.SendAsync<EzspEchoRequest, EzspEchoResponse>(EzspCommand.Echo, new EzspEchoRequest {Data = text});
        return response.Data;
    }

    public async Task<EzspResponse> SetConfigurationValueAsync(EzspConfigId configId, ushort value)
    {
        return await Channel.SendAsync<EzspSetConfigurationValueRequest, EzspResponse>(EzspCommand.SetConfigurationValue, new EzspSetConfigurationValueRequest { ConfigId = configId, Value = value });
    }

    public async Task<EzspGetConfigurationValueResponse> GetConfigurationValueAsync(EzspConfigId configId)
    {
        return await Channel.SendAsync<EzspGetConfigurationValueRequest, EzspGetConfigurationValueResponse> (EzspCommand.SetConfigurationValue, new EzspGetConfigurationValueRequest { configId = configId });
    }
}