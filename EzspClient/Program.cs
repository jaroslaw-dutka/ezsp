using System.Net.Sockets;
using EzspLib;
using EzspLib.Ezsp;
using EzspLib.Model;

var tcp = new TcpClient();
tcp.Connect("192.168.1.40", 8888);
var channel = new EzspChannel(tcp.GetStream());
await channel.ConnectAsync(CancellationToken.None);

channel.CallbackReceived += bytes =>
{
    Console.WriteLine("Callback");
    var aa = 2;
};

var ezsp = new EzspClient(channel);

// await ezsp.EchoAsync("test");
// await ezsp.SetMfgTokenAsync(EzspMfgTokenId.MFG_BOARD_NAME, "dupadupadupablad");
// var aaa = await ezsp.GetMfgTokenAsync(EzspMfgTokenId.MFG_BOARD_NAME);
// var bbb = await ezsp.GetMfgTokenAsync(EzspMfgTokenId.MFG_MANUF_ID);
// await channel.SendAsync<EzspResponse>(EzspCommand.GetNumStoredBeacons);
// var beaconResponse = await channel.SendAsync<EzspGetFirstBeaconResponse>(EzspCommand.GetFirstBeacon);

var version = await ezsp.VersionAsync(7);
await ezsp.SetConfigurationValueAsync(EzspConfigId.STACK_PROFILE, version.StackType);
await ezsp.SetConfigurationValueAsync(EzspConfigId.SECURITY_LEVEL, 5);
await ezsp.SetConfigurationValueAsync(EzspConfigId.SUPPORTED_NETWORKS, 1);
await ezsp.SetConfigurationValueAsync(EzspConfigId.PACKET_BUFFER_COUNT, 64);

var response = await ezsp.NetworkInitAsync();
if ((byte)response.Status != 0x93) 
    response = await ezsp.LeaveNetworkAsync();

var state = await ezsp.NetworkStateAsync();
while (state.Status != EmberNetworkStatus.NO_NETWORK)
{
    await Task.Delay(100);
    state = await ezsp.NetworkStateAsync();
}

await ezsp.SetInitialSecurityStateAsync(new EmberInitialSecurityState
{
    Bitmask = EmberInitialSecurity.HAVE_PRECONFIGURED_KEY | EmberInitialSecurity.HAVE_NETWORK_KEY | EmberInitialSecurity.REQUIRE_ENCRYPTED_KEY | EmberInitialSecurity.TRUST_CENTER_GLOBAL_LINK_KEY,
    NetworkKey = new EmberKeyData(0xFF),
    PreconfiguredKey = new EmberKeyData("ZigBeeAlliance09")
});

await ezsp.StartScanAsync(EzspNetworkScanType.ACTIVE_SCAN, 0x07FFF800, 5);
await Task.Delay(5000);
await ezsp.StopScanAsync();

await ezsp.JoinNetworkAsync(EmberNodeType.ROUTER, new EmberNetworkParameters
{
    ExtendedPanId = new EzspExtendedPanId(0x00124B0029DDECFB),
    PanId = 0x1A62,
    RadioTxPower = 8,
    RadioChannel = 11,
    JoinMethod = EmberJoinMethod.USE_MAC_ASSOCIATION,
    NwkManagerId = 0,
    NwkUpdateId = 0,
    Channels = 0
});

await Task.Delay(200000);