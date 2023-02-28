using EzspLib.Ezsp;
using EzspLib.Model;
using EzspLib.Requests;

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

    public async Task<EzspVersionResponse> VersionAsync(byte version)
    {
        return await Channel.SendAsync<EzspVersionRequest, EzspVersionResponse>(EzspCommand.Version, new EzspVersionRequest { EzspVersion = version });
    }

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

    public async Task<EzspResponse> SetInitialSecurityStateAsync(EmberInitialSecurityState state)
    {
        return await Channel.SendAsync<EzspSetInitialSecurityStateRequest, EzspResponse>(EzspCommand.SetInitialSecurityState, new EzspSetInitialSecurityStateRequest
        {
            State = state
        });
    }

    public async Task<EzspResponse> NetworkInitAsync()
    {
        return await Channel.SendAsync<EzspResponse>(EzspCommand.NetworkInit);
    }

    public async Task<EzspNetworkStateResponse> NetworkStateAsync()
    {
        return await Channel.SendAsync<EzspNetworkStateResponse>(EzspCommand.NetworkState);
    }

    public async Task<EzspResponse> LeaveNetworkAsync()
    {
        return await Channel.SendAsync<EzspResponse>(EzspCommand.LeaveNetwork);
    }

    public async Task<EzspResponse> JoinNetworkAsync(EmberNodeType nodeType, EmberNetworkParameters parameters)
    {
        return await Channel.SendAsync<EzspJoinNetworkRequest, EzspResponse>(EzspCommand.JoinNetwork, new EzspJoinNetworkRequest
        {
            NodeType = nodeType,
            Parameters = parameters
        });
    }

    public async Task<EzspResponse> StartScanAsync(EzspNetworkScanType scanType, uint channelMask, byte duration)
    {
        return await Channel.SendAsync<EzspStartScanRequest, EzspResponse>(EzspCommand.StartScan, new EzspStartScanRequest
        {
            ScanType = scanType,
            ChannelMask = channelMask,
            Duration = duration
        });
    }

    public async Task<EzspResponse> StopScanAsync()
    {
        return await Channel.SendAsync<EzspResponse>(EzspCommand.StopScan);
    }

    public async Task LaunchStandaloneBootloaderAsync()
    {
        await Channel.SendAsync(EzspCommand.LaunchStandaloneBootloader);
    }

    public async Task SetTimerAsync()
    {
        // TODO:
        await Channel.SendAsync(EzspCommand.SetTimer, 0, 100, 0, 1, 1);
    }

    public async Task NopAsync()
    {
        // TODO:
        await Channel.SendAsync(EzspCommand.Nop);
    }

    public async Task DelayTestAsync()
    {
        // TODO:
        //await client.SendAsync(new byte[] { index++, 0, 1, 0x9D, 0, 0, 10 });
        await Channel.SendAsync(EzspCommand.DelayTest);
    }

    public async Task<string> GetMfgTokenAsync(EzspMfgTokenId tokenId)
    {
        var response = await Channel.SendAsync<EzspGetMfgTokenRequest, EzspGetMfgTokenResponse>(EzspCommand.GetMfgToken, new EzspGetMfgTokenRequest
        {
            TokenId = tokenId
        });
        return response.Data;
    }

    public async Task<EzspResponse> SetMfgTokenAsync(EzspMfgTokenId tokenId, string data)
    {
        return await Channel.SendAsync<EzspSetMfgTokenRequest, EzspResponse>(EzspCommand.SetMfgToken, new EzspSetMfgTokenRequest
        {
            TokenId = tokenId,
            Data = data
        });
    }
}