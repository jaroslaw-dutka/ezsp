using System.Buffers.Binary;
using System.Text.Json;
using System.Text.Json.Serialization;
using Bitjuice.EmberZNet.Api;
using Bitjuice.EmberZNet.Model;

namespace Bitjuice.EmberZNet.Client;

public class TestApp: IEzspCallbackHandler
{
    private readonly EzspApi ezsp;

    public TestApp(Stream stream)
    {
        ezsp = new EzspApi(stream);
    }

    public async Task RunAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("Connecting");
        await ezsp.Channel.ConnectAsync(this, cancellationToken);

        await ConfigureAsync();
        await SetEndpoints();

        var status = await InitNetworkAsync();
        if (status == EmberStatus.NotJoined)
        {
            // await LeaveNetwork();
            // await ScanAsync();
            await InitSecurityAsync();
            await JoinNetworkAsync();
        }

        cancellationToken.WaitHandle.WaitOne();

        Console.WriteLine("Disconnecting");
        await ezsp.Channel.DisconnectAsync(CancellationToken.None);

        Console.WriteLine("Disconnected");
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
        var zzz = await ezsp.Channel.SendAsync<EzspAddEndpointRequest, EzspAddEndpointResponse>(EzspCommand.AddEndpoint, new EzspAddEndpointRequest
        {
            Endpoint = 1,
            ProfileId = ZigBeeProfileId.ZigbeeHomeAutomation,
            DeviceId = ZigBeeDeviceId.OnOffSwitch,
            InputClusterList = new ushort[] { (byte)ZigBeeClusterId.GenIdentify, (byte)ZigBeeClusterId.GenOnOff },
            OutputClusterList = new ushort[] { }
        });

        var yyy = await ezsp.Channel.SendAsync<EzspAddEndpointRequest, EzspAddEndpointResponse>(EzspCommand.AddEndpoint, new EzspAddEndpointRequest
        {
            Endpoint = 3,
            ProfileId = ZigBeeProfileId.ZigbeeHomeAutomation,
            DeviceId = ZigBeeDeviceId.OnOffSwitch,
            InputClusterList = new ushort[] { (byte)ZigBeeClusterId.GenIdentify },
            OutputClusterList = new ushort[] { }
        });
    }

    private async Task<EmberStatus> InitNetworkAsync()
    {
        var response = await ezsp.NetworkInitAsync();
        Console.WriteLine("Network status: " + response.Status);
        return response.Status;
    }

    private async Task LeaveNetwork()
    {
        Console.WriteLine("Leaving network");
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
        Console.WriteLine("Joining network");
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
            WriteIndented = false,
            Converters = { new JsonStringEnumConverter() }
        });
        Console.WriteLine(json);
    }
}