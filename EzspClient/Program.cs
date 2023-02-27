using System.Net.Sockets;
using System.Text;
using EzspLib.Ezsp;
using EzspLib.Model;

var tcp = new TcpClient();
tcp.Connect("192.168.1.40", 8888);
var client = new EzspChannel(tcp.GetStream());

await client.ConnectAsync(CancellationToken.None);

// await client.SendAsync(EzspCommand.Version, 7);
// Echo
// await client.SendAsync(EzspCommand.Echo, 3, 1, 2, 3);
// Bootloader
// await client.SendSync(new byte[] { index++, 0, 1, 0x8f, 0, 0 });
// setTimer
// await client.SendAsync(EzspCommand.SetTimer, 0, 100, 0, 1, 1);
// NOP
// await client.SendAsync(EzspCommand.Nop);
// Delay
//await client.SendAsync(new byte[] { index++, 0, 1, 0x9D, 0, 0, 10 });
// for (var i = 0; i < 20; i++)
// {
//     var response = await client.SendAsync(EzspCommand.Version, 7);
//     if (response.Span[0] != 0x07)
//         throw new Exception("aaa");
// }

// await Task.Delay(1000);
// await client.ConnectAsync(CancellationToken.None);
// await client.SendAsync(EzspCommand.Echo, 3, 1, 2, 3);

// var status = await client.SendAsync<EzspRequest, EzspResponse>(EzspCommand.NetworkState, new EzspRequest());

await client.SendAsync<EzspSetConfigurationValueRequest, EzspResponse>(EzspCommand.SetConfigurationValue, new EzspSetConfigurationValueRequest()
{
    configId = EzspConfigId.EZSP_CONFIG_STACK_PROFILE,
    value = 2
});
// await client.SendAsync<EzspSetConfigurationValueRequest, EzspResponse>(EzspCommand.SetConfigurationValue, new EzspSetConfigurationValueRequest()
// {
//     configId = EzspConfigId.EZSP_CONFIG_SECURITY_LEVEL,
//     value = 5
// });
// await client.SendAsync<EzspSetConfigurationValueRequest, EzspResponse>(EzspCommand.SetConfigurationValue, new EzspSetConfigurationValueRequest()
// {
//     configId = EzspConfigId.EZSP_CONFIG_SUPPORTED_NETWORKS,
//     value = 1
// });
// await client.SendAsync<EzspSetConfigurationValueRequest, EzspResponse>(EzspCommand.SetConfigurationValue, new EzspSetConfigurationValueRequest()
// {
//     configId = EzspConfigId.EZSP_CONFIG_PACKET_BUFFER_COUNT,
//     value = 64
// });


var response = await client.SendAsync<EzspRequest, EzspResponse>(EzspCommand.NetworkInit, new EzspRequest());

if ((byte)response.status != 0x93)
{
    await client.SendAsync<EzspRequest, EzspResponse>(EzspCommand.LeaveNetwork, new EzspRequest());
}

var aaa = new string("ZigBeeAl");
var bbb = new string("liance09");

var securityResponse = await client.SendAsync<EzspSetInitialSecurityStateRequest, EzspResponse>(EzspCommand.SetInitialSecurityState, new EzspSetInitialSecurityStateRequest
{
    state = new EmberInitialSecurityState
    {
        bitmask = EmberInitialSecurityBitmask.EMBER_HAVE_PRECONFIGURED_KEY
                   | EmberInitialSecurityBitmask.EMBER_HAVE_NETWORK_KEY
                   | EmberInitialSecurityBitmask.EMBER_REQUIRE_ENCRYPTED_KEY
                   | EmberInitialSecurityBitmask.EMBER_TRUST_CENTER_GLOBAL_LINK_KEY,
        networkKey = new EmberKeyData
        {
            // field1 = 0x00020406080a0c0e,
            // field2 = 0x01030507090b0d0f
            field1 = 0xffffffffffffffff,
            field2 = 0xffffffffffffffff
        },
        preconfiguredKey = new EmberKeyData
        {
            field1 = BitConverter.ToUInt64(Encoding.ASCII.GetBytes(aaa)),
            field2 = BitConverter.ToUInt64(Encoding.ASCII.GetBytes(bbb)),
        }
        // preconfiguredTrustCenterEui64 = 
    }
});

//
// await client.SendAsync<EzspStartScanRequest, EzspResponse>(EzspCommand.StartScan, new EzspStartScanRequest
// {
//     scanType = EzspNetworkScanType.EZSP_ACTIVE_SCAN,
//     channelMask = 0x07FFF800,
//     duration = 5
// });
// await Task.Delay(5000);
// await client.SendAsync<EzspRequest, EzspResponse>(EzspCommand.StopScan, new EzspRequest());

// await client.SendAsync<EzspRequest, EzspResponse>(EzspCommand.GetNumStoredBeacons, new EzspRequest());
// return;

//
// var beaconResponse = await client.SendAsync<EzspRequest, EzspGetFirstBeaconResponse>(EzspCommand.GetFirstBeacon, new EzspRequest());

var joinResponse = await client.SendAsync<EzspJoinNetworkRequest, EzspResponse>(EzspCommand.JoinNetwork, new EzspJoinNetworkRequest
{
    nodeType = EmberNodeType.EMBER_END_DEVICE,
    parameters = new EmberNetworkParameters
    {
        extendedPanId = 0x00124B0029DDECFB,
        panId = 0x1A62,
        radioTxPower = 8,
        radioChannel = 11,
        joinMethod = EmberJoinMethod.EMBER_USE_MAC_ASSOCIATION,
        nwkManagerId = 0,
        nwkUpdateId = 0,
        //channels = 0x07FFF800
        channels = 0
    }
});

//
// var response2 = await client.SendAsync(EzspCommand.JoinNetwork, new byte[]
// {
//     0x02, // router
//     //0x00, 0x12, 0x4B, 0x00, 0x29, 0xDD, 0xEC, 0xFB,     // extended PAN ID
//     //0x1A, 0x62,                                         // PAN ID
//     0xfb, 0xec, 0xdd, 0x29, 0x00, 0x4b, 0x12, 0x00,
//     0x62, 0x1A,
//     0x11, // A power setting, in dBm  1 byte
//     0x11, // A radio channel.
//     0x00, // The method used to initially join the network.
//     0x00, // NWK Update ID.
//     0x00, 0x00, 0x00, 0x00 // NWK channel mask.        4 bytes
// });

await Task.Delay(200000);