
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Tide.Ork.Models;

namespace Tide.Ork.Classes
{
    public class SendGridEmailClient : IEmailClient
    {
        private readonly Settings _settings;

        public SendGridEmailClient(Settings settings)
        {
            _settings = settings;
        }

        public bool SendEmail(string recipient, string recipientEmail, string subject, string content) {
            var client = new SendGridClient(_settings.EmailClient.Key);
            var from = new EmailAddress(_settings.EmailClient.SenderEmail, _settings.EmailClient.SenderName);
            var to = new EmailAddress(recipientEmail, recipient);

            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, "");
            var response = client.SendEmailAsync(msg).Result;
            return response.IsSuccessStatusCode;
        }
    }
}
