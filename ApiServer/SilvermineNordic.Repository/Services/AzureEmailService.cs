
using Azure.Communication.Email;
using Azure.Communication.Email.Models;

namespace SilvermineNordic.Repository.Services
{
    public class AzureEmailService : IEmailService
    {
        private readonly ISilvermineNordicConfiguration _configuration;
        public AzureEmailService(ISilvermineNordicConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            EmailClient emailClient = new EmailClient(_configuration.GetEmailServiceConnectionString());
            var content = new EmailContent(subject)
            {
                PlainText = body,
                Html = body,
            };
            var recipients = new EmailRecipients(new List<EmailAddress>() { new EmailAddress(to) { DisplayName = "Brandon Miller" } });
            try
            {
                var response = await emailClient.SendAsync(
                    new EmailMessage("DoNotReply@e5f9f136-3489-4829-929b-5a095c48d5eb.azurecomm.net",
                        content,
                        recipients), CancellationToken.None);
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }
    }
}
