using System.Buffers.Binary;
using System.Net.Sockets;
using Bitjuice.EmberZNet.Api;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Client;

public class TestApp
{
    private EzspApi ezsp;

    public async Task ConnectAsync()
    {
        var tcp = new TcpClient();
        await tcp.ConnectAsync("192.168.1.40", 8888);
        var channel = new EzspChannel(tcp.GetStream());
        await channel.ConnectAsync(CancellationToken.None);
        channel.CallbackReceived += CallbackReceived;
        ezsp = new EzspApi(channel);
    }

    public async Task RunAsync()
    {
        await ezsp.SetTimerAsync(1);
        for(var i=0; i<100; i++)
            ezsp.EchoAsync("test");

        // await ezsp.SetMfgTokenAsync(EzspMfgTokenId.MFG_BOARD_NAME, "dupadupadupablad");
        // var aaa = await ezsp.GetMfgTokenAsync(EzspMfgTokenId.MFG_BOARD_NAME);
        // var bbb = await ezsp.GetMfgTokenAsync(EzspMfgTokenId.MFG_MANUF_ID);
        // await channel.SendAsync<EzspResponse>(EzspCommand.GetNumStoredBeacons);
        // var beaconResponse = await channel.SendAsync<EzspGetFirstBeaconResponse>(EzspCommand.GetFirstBeacon);

        // await ConfigureAsync();
        // await InitNetworkAsync();
        // await InitSecurityAsync();
        // await ScanAsync();
        // await JoinNetworkAsync();
    }

    private async Task ConfigureAsync()
    {
        var version = await ezsp.VersionAsync(7);
        await ezsp.SetConfigurationValueAsync(EzspConfigId.StackProfile, version.StackType);
        await ezsp.SetConfigurationValueAsync(EzspConfigId.SecurityLevel, 5);
        await ezsp.SetConfigurationValueAsync(EzspConfigId.SupportedNetworks, 1);
        await ezsp.SetConfigurationValueAsync(EzspConfigId.PacketBufferCount, 64);

        var zzz = await ezsp.Channel.SendAsync<EzspAddEndpointRequest, EzspAddEndpointResponse>(EzspCommand.AddEndpoint, new EzspAddEndpointRequest()
        {
            Endpoint = 1,
            ProfileId = 0,
            DeviceId = 0,
            InputClusterCount = 3,
            InputClusterList = new ushort[] { 0, 3, 6 },
            OutputClusterCount = 1,
            OutputClusterList = new ushort[] { 10 }
        });
        // var xxx = await channel.SendAsync<EzspAddEndpointRequest, EzspAddEndpointResponse>(EzspCommand.AddEndpoint, new EzspAddEndpointRequest()
        // {
        //     Endpoint = 2,
        //     ProfileId = 0,
        //     DeviceId = 0,
        //     InputClusterCount = 1,
        //     InputClusterList = new ushort[] { 0, 3 },
        //     OutputClusterCount = 1,
        //     OutputClusterList = new ushort[] { 1 }
        // });
    }

    private async Task InitNetworkAsync()
    {
        var response = await ezsp.NetworkInitAsync();
        if ((byte)response.Status != 0x93)
            response = await ezsp.LeaveNetworkAsync();

        var state = await ezsp.NetworkStateAsync();
        while (state.Status != EmberNetworkStatus.NoNetwork)
        {
            await Task.Delay(100);
            state = await ezsp.NetworkStateAsync();
        }
    }

    private async Task InitSecurityAsync()
    {
        await ezsp.SetInitialSecurityStateAsync(new EmberInitialSecurityState
        {
            Bitmask = EmberInitialSecurity.HavePreconfiguredKey | EmberInitialSecurity.HaveNetworkKey | EmberInitialSecurity.RequireEncryptedKey | EmberInitialSecurity.TrustCenterGlobalLinkKey,
            NetworkKey = new EmberKeyData(0xFF),
            PreconfiguredKey = new EmberKeyData("ZigBeeAlliance09")
        });
    }

    private async Task ScanAsync()
    {
        await ezsp.StartScanAsync(EzspNetworkScanType.ActiveScan, 0x07FFF800, 5);
        await Task.Delay(5000);
        await ezsp.StopScanAsync();
    }

    private async Task JoinNetworkAsync()
    {
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
    }

    private void CallbackReceived(byte[] bytes)
    {
        var cmd = (EzspCommand)BinaryPrimitives.ReadUInt16BigEndian(bytes.AsSpan(3));

        Console.WriteLine($"Callback {cmd}");
        Console.WriteLine();

        if (cmd == EzspCommand.StackStatusHandler)
        {
            var aa = EzspSerializer.Deserialize<EzspStackStatusHandlerResponse>(bytes.AsSpan(5).ToArray());
            if (aa.Status != EmberStatus.Success)
                Task.Delay(2000).ContinueWith(async (obj) =>
                {
                    await JoinNetworkAsync();
                });
        }
        if (cmd == EzspCommand.IncomingMessageHandler)
        {
            var aa = EzspSerializer.Deserialize<EzspIncomingMessageHandlerResponse>(bytes.AsSpan(5).ToArray());
            var bb = 2;
        }

        var cc = 2;
    }
}