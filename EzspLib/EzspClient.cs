using EzspLib.Ezsp;
using EzspLib.Model;

namespace EzspLib
{
    public class EzspClient
    {
        private EzspChannel Channel { get; }

        public EzspClient(Stream stream)
        {
            Channel = new EzspChannel(stream);
        }

        public async Task<EzspStatus> SetConfigurationValueAsync(EzspConfigId configId, ushort value)
        {
            var request = new EzspSetConfigurationValueRequest { configId = configId, value = value };
            var response = await Channel.SendAsync<EzspSetConfigurationValueRequest, EzspResponse>(EzspCommand.SetConfigurationValue, request);
            return response.status;
        }

        //public async Task<EzspStatus> GetConfigurationValueAsync(EzspConfigId configId, out ushort value)
        //{
        //    var request = new EzspGetConfigurationValueRequest { configId = configId };
        //    var response = await Channel.SendAsync<EzspGetConfigurationValueRequest, EzspGetConfigurationValueResponse> (EzspCommand.SetConfigurationValue, request);
        //    value = response.value;
        //    return response.status;
        //}
    }
}
