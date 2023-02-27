using EzspLib.Ezsp;
using EzspLib.Model;

namespace EzspLib
{
    public class EzspClient
    {
        private readonly EzspChannel Channel { get; }

        public EzspClient(Stream stream)
        {
            Channel = new EzspChannel(stream);
        }

        public async Task<EmberStatus> SetConfigurationValueAsync(EzspConfigId configId, ushort value)
        {
            var request = new EzspSetConfigurationValueRequest { configId = configId, value = value };
            var response = await Channel.SendAsync<EzspSetConfigurationValueRequest, EzspResponse>(EzspCommand.SetConfigurationValue, request);
            return response.status;
        }
    }
}
