using System.Net.Sockets;
using EzspLib;
using EzspLib.Ezsp;
using EzspLib.Model;

var tcp = new TcpClient();
tcp.Connect("192.168.1.40", 8888);
var channel = new EzspChannel(tcp.GetStream());

await channel.ConnectAsync(CancellationToken.None);

var ezsp = new EzspClient(channel);


await ezsp.SetConfigurationValueAsync(EzspConfigId.EZSP_CONFIG_STACK_PROFILE, 2);
await ezsp.SetConfigurationValueAsync(EzspConfigId.EZSP_CONFIG_SECURITY_LEVEL, 5);
await ezsp.SetConfigurationValueAsync(EzspConfigId.EZSP_CONFIG_SUPPORTED_NETWORKS, 1);
await ezsp.SetConfigurationValueAsync(EzspConfigId.EZSP_CONFIG_PACKET_BUFFER_COUNT, 64);
await ezsp.EchoAsync("test");

// await client.SendAsync(EzspCommand.Version, 7);

// Bootloader
// await client.SendSync(new byte[] { index++, 0, 1, 0x8f, 0, 0 });
// setTimer
// await client.SendAsync(EzspCommand.SetTimer, 0, 100, 0, 1, 1);
// NOP
// await client.SendAsync(EzspCommand.Nop);
// Delay
//await client.SendAsync(new byte[] { index++, 0, 1, 0x9D, 0, 0, 10 });
// await client.ConnectAsync(CancellationToken.None);
// var status = await client.SendAsync<EzspRequest, EzspResponse>(EzspCommand.NetworkState, new EzspRequest());
// await channel.SendAsync<EzspSetConfigurationValueRequest, EzspResponse>(EzspCommand.SetConfigurationValue, new EzspSetConfigurationValueRequest
// {
//     ConfigId = EzspConfigId.EZSP_CONFIG_STACK_PROFILE,
//     Value = 2
// });
// var buffer = new byte[18];
// buffer[0] = 2;
// buffer[1] = 16;
// "testtest"u8.ToArray().CopyTo(buffer, 2);
// await channel.SendAsync(EzspCommand.SetMfgToken, buffer);
// var aaa = await channel.SendAsync(EzspCommand.GetMfgToken, 1);

var response = await channel.SendAsync<EzspResponse>(EzspCommand.NetworkInit);
if ((byte)response.Status != 0x93) 
    response = await channel.SendAsync<EzspResponse>(EzspCommand.LeaveNetwork);

var securityResponse = await channel.SendAsync<EzspSetInitialSecurityStateRequest, EzspResponse>(EzspCommand.SetInitialSecurityState, new EzspSetInitialSecurityStateRequest
{
    State = new EmberInitialSecurityState
    {
        Bitmask = EmberInitialSecurityBitmask.EMBER_HAVE_PRECONFIGURED_KEY
                  | EmberInitialSecurityBitmask.EMBER_HAVE_NETWORK_KEY
                  | EmberInitialSecurityBitmask.EMBER_REQUIRE_ENCRYPTED_KEY
                  | EmberInitialSecurityBitmask.EMBER_TRUST_CENTER_GLOBAL_LINK_KEY,
        NetworkKey = new EmberKeyData(0xFF),
        PreconfiguredKey = new EmberKeyData("ZigBeeAlliance09")
    }
});

// await channel.SendAsync<EzspStartScanRequest, EzspResponse>(EzspCommand.StartScan, new EzspStartScanRequest
// {
//     ScanType = EzspNetworkScanType.EZSP_ACTIVE_SCAN,
//     ChannelMask = 0x07FFF800,
//     Duration = 5
// });
// await Task.Delay(5000);
// await channel.SendAsync<EzspResponse>(EzspCommand.StopScan);
// await channel.SendAsync<EzspResponse>(EzspCommand.GetNumStoredBeacons);
// var beaconResponse = await channel.SendAsync<EzspGetFirstBeaconResponse>(EzspCommand.GetFirstBeacon);

var joinResponse = await channel.SendAsync<EzspJoinNetworkRequest, EzspResponse>(EzspCommand.JoinNetwork, new EzspJoinNetworkRequest
{
    NodeType = EmberNodeType.EMBER_ROUTER,
    Parameters = new EmberNetworkParameters
    {
        ExtendedPanId = new EzspExtendedPanId(0x00124B0029DDECFB),
        PanId = 0x1A62,
        RadioTxPower = 8,
        RadioChannel = 11,
        JoinMethod = EmberJoinMethod.EMBER_USE_MAC_ASSOCIATION,
        NwkManagerId = 0,
        NwkUpdateId = 0,
        Channels = 0
    }
});

await Task.Delay(200000);