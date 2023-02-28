using System.Net.Sockets;
using Bitjuice.EmberZNet;
using Bitjuice.EmberZNet.Model;

var tcp = new TcpClient();
tcp.Connect("192.168.1.40", 8888);
var channel = new EzspChannel(tcp.GetStream());
await channel.ConnectAsync(CancellationToken.None);

channel.CallbackReceived += bytes =>
{
    Console.WriteLine("Callback");
    var aa = 2;
};

var ezsp = new EzspApi(channel);

// await ezsp.EchoAsync("test");
// await ezsp.SetMfgTokenAsync(EzspMfgTokenId.MFG_BOARD_NAME, "dupadupadupablad");
// var aaa = await ezsp.GetMfgTokenAsync(EzspMfgTokenId.MFG_BOARD_NAME);
// var bbb = await ezsp.GetMfgTokenAsync(EzspMfgTokenId.MFG_MANUF_ID);
// await channel.SendAsync<EzspResponse>(EzspCommand.GetNumStoredBeacons);
// var beaconResponse = await channel.SendAsync<EzspGetFirstBeaconResponse>(EzspCommand.GetFirstBeacon);

var version = await ezsp.VersionAsync(7);
await ezsp.SetConfigurationValueAsync(EzspConfigId.StackProfile, version.StackType);
await ezsp.SetConfigurationValueAsync(EzspConfigId.SecurityLevel, 5);
await ezsp.SetConfigurationValueAsync(EzspConfigId.SupportedNetworks, 1);
await ezsp.SetConfigurationValueAsync(EzspConfigId.PacketBufferCount, 64);

var response = await ezsp.NetworkInitAsync();
if ((byte)response.Status != 0x93) 
    response = await ezsp.LeaveNetworkAsync();

var state = await ezsp.NetworkStateAsync();
while (state.Status != EmberNetworkStatus.NoNetwork)
{
    await Task.Delay(100);
    state = await ezsp.NetworkStateAsync();
}

await ezsp.SetInitialSecurityStateAsync(new EmberInitialSecurityState
{
    Bitmask = EmberInitialSecurity.HavePreconfiguredKey | EmberInitialSecurity.HaveNetworkKey | EmberInitialSecurity.RequireEncryptedKey | EmberInitialSecurity.TrustCenterGlobalLinkKey,
    NetworkKey = new EmberKeyData(0xFF),
    PreconfiguredKey = new EmberKeyData("ZigBeeAlliance09")
});

await ezsp.StartScanAsync(EzspNetworkScanType.ActiveScan, 0x07FFF800, 5);
await Task.Delay(5000);
await ezsp.StopScanAsync();

await ezsp.JoinNetworkAsync(EmberNodeType.Router, new EmberNetworkParameters
{
    ExtendedPanId = new EzspExtendedPanId(0x00124B0029DDECFB),
    PanId = 0x1A62,
    RadioTxPower = 8,
    RadioChannel = 11,
    JoinMethod = EmberJoinMethod.UseMacAssociation,
    NwkManagerId = 0,
    NwkUpdateId = 0,
    Channels = 0
});

await Task.Delay(100000);