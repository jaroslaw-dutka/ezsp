using System.Buffers.Binary;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bitjuice.EmberZNet.Api;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Client;

public class TestApp: IEzspCallbackHandler
{
    private EzspApi ezsp;

    public async Task ConnectAsync(CancellationToken cancellationToken)
    {
        var tcp = new TcpClient();
        await tcp.ConnectAsync("192.168.1.40", 8888, cancellationToken);
        ezsp = new EzspApi(tcp.GetStream(), this);
        Console.WriteLine("Connecting");
        await ezsp.Channel.ConnectAsync(cancellationToken);
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        await ConfigureAsync();
        await SetEndpoints();
        var status = await InitNetworkAsync();
        if (status == EmberStatus.NotJoined)
        {
            Console.WriteLine("Joining network");
            // await LeaveNetwork();
            // await ScanAsync();
            await InitSecurityAsync();
            await JoinNetworkAsync();
        }
    }

    private async Task ConfigureAsync()
    {
        Console.WriteLine("Configuring module");
        var version = await ezsp.VersionAsync(7);
        await ezsp.SetConfigurationValueAsync(EzspConfigId.StackProfile, version.StackType);
        await ezsp.SetConfigurationValueAsync(EzspConfigId.SecurityLevel, 5);
        await ezsp.SetConfigurationValueAsync(EzspConfigId.SupportedNetworks, 1);
        await ezsp.SetConfigurationValueAsync(EzspConfigId.PacketBufferCount, 128);
    }

    private async Task SetEndpoints()
    {
        Console.WriteLine("Setting endpoints");
        var zzz = await ezsp.Channel.SendAsync<EzspAddEndpointRequest, EzspAddEndpointResponse>(EzspCommand.AddEndpoint, new EzspAddEndpointRequest()
        {
            Endpoint = 1,
            ProfileId = ZigBeeProfileId.ZigbeeHomeAutomation,
            DeviceId = ZigBeeDeviceId.OnOffSwitch,
            InputClusterCount = 1,
            InputClusterList = new ushort[] { 0 },
            OutputClusterCount = 1,
            OutputClusterList = new ushort[] { 0 }
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

    private async Task<EmberStatus> InitNetworkAsync()
    {
        var response = await ezsp.NetworkInitAsync();
        Console.WriteLine("Network status: " + response.Status);
        return response.Status;
    }

    private async Task LeaveNetwork()
    {
        await ezsp.LeaveNetworkAsync();
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

    public async Task HandleCallbackAsync(byte[] data)
    {
        var cmd = (EzspCommand)BinaryPrimitives.ReadUInt16BigEndian(data.AsSpan(3));

        Console.WriteLine($"Callback {cmd}");

        switch (cmd)
        {
            case EzspCommand.StackStatusHandler:
            {
                var aa = EzspSerializer.Deserialize<EzspStackStatusHandlerResponse>(data.AsSpan(5).ToArray());
                Dump(aa);

                if (aa.Status == EmberStatus.JoinFailed)
                {
                    await Task.Delay(2000);
                    await JoinNetworkAsync();
                }

                if (aa.Status == EmberStatus.NetworkUp)
                {
                    // var networkParams = ezsp.Channel.SendAsync(EzspCommand.GetNetworkParameters);
                }

                break;
            }
            case EzspCommand.IncomingMessageHandler:
            {
                var aa = EzspSerializer.Deserialize<EzspIncomingMessageHandlerResponse>(data.AsSpan(5).ToArray());
                Dump(aa);

                // Console.WriteLine(BitConverter.ToString(aa.MessageContents).Replace("-", " "));
            
                var bb = 2;
                break;
            }
        }

        Console.WriteLine();
    }

    private static void Dump(object obj)
    {
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter() }
        });
        Console.WriteLine(json);
    }
}