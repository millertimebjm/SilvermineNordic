using Azure.Communication.Sms;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public class AzureSmsService : ISms
    {
        private readonly string _azureSmsConnectionString;
        private readonly string _azureSmsFromPhone;
        private readonly ISilvermineNordicConfiguration _configuration;
        public AzureSmsService(
            IOptionsSnapshot<SilvermineNordicConfigurationService> options)
        {
            _configuration = options.Value;
            _azureSmsConnectionString = _configuration.GetAzureSmsConnectionString();
            _azureSmsFromPhone = _configuration.GetAzureSmsFromPhone();
        }

        public async Task<bool> SendSms(string to, string message)
        {
            try
            {
                var smsClient = new SmsClient(_azureSmsConnectionString);
                SmsSendResult sendResult = await smsClient.SendAsync(
                    from: _azureSmsFromPhone,
                    to: to,
                    message: message
                );
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }
    }
}
