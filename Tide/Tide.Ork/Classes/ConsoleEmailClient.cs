using System;
using Tide.Ork.Models;

namespace Tide.Ork.Classes
{
    public class ConsoleEmailClient : IEmailClient
    {
        private readonly Settings _settings;

        private string SenderName => _settings.EmailClient.SenderName;
        private string SenderEmail => _settings.EmailClient.SenderEmail;
        private string Username => _settings.Instance.Username;

        public ConsoleEmailClient(Settings settings) => _settings = settings;

        public bool SendEmail(string recipient, string recipientEmail, string subject, string content) {
            Console.WriteLine("-----BEGIN EMAIL-----");
            Console.WriteLine($"Sender: {Username}");
            Console.WriteLine($"From: {SenderName}<{SenderEmail}>");
            Console.WriteLine($"To: {recipient}<{recipientEmail}>");
            Console.WriteLine($"Date: {DateTime.Now}");
            Console.WriteLine($"Subject: {subject}");
            Console.WriteLine("Content-Type: text/plain");
            Console.WriteLine();
            Console.BackgroundColor = ConsoleColor.Blue;
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(content);
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("-----END EMAIL-----");

            return true;
        }
    }
}
