using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;
using Tide.Ork.Models;

namespace Tide.Ork.Classes
{
    public class MailKitClient : IEmailClient
    {
        private readonly Settings _settings;

        public MailKitClient(Settings settings) {
            _settings = settings;
        }

        public bool SendEmail(string recipient, string recipientEmail, string subject, string content) {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_settings.EmailClient.SenderName, _settings.EmailClient.SenderEmail));
                message.To.Add(new MailboxAddress(recipient, recipientEmail));
                message.Subject = subject;
                message.Body = new TextPart("plain")
                {
                    Text = content
                };

                using (var client = new SmtpClient())
                {
                    client.Connect(_settings.EmailClient.SenderHost, _settings.EmailClient.SenderPort, false);
                    client.Authenticate(_settings.EmailClient.SenderEmail, _settings.EmailClient.SenderPassword);
                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception) {
                return false;
            }

            return true;
        }
    }
}
