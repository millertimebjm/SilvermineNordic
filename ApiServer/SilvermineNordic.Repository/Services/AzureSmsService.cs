using Azure.Communication.Sms;
using System;
using System.Threading.Tasks;

namespace SilvermineNordic.Repository.Services
{
    public class AzureSmsService : ISms
    {
        private readonly string _azureSmsConnectionString;
        private readonly string _azureSmsFromPhone;
        public AzureSmsService(ISilvermineNordicConfiguration configuration) 
        {
            _azureSmsConnectionString = configuration.GetAzureSmsConnectionString();
            _azureSmsFromPhone = configuration.GetAzureSmsFromPhone();
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
