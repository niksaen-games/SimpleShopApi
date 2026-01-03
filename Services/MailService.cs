using MailKit.Net.Smtp;
using MimeKit;

namespace SimpleShopApi.Services
{
    public class MailService(IConfiguration configuration)
    {
        private async Task SendHtmlEmailAsync(string to, string subject, string body)
        {
            var smtpConfig = configuration.GetSection("SmtpClient");

            using var client = new SmtpClient();
            await client.ConnectAsync(smtpConfig["Host"], int.Parse(smtpConfig["Port"] ?? "587"), false);
            await client.AuthenticateAsync(smtpConfig["User"], smtpConfig["AppPassword"]);

            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(smtpConfig["User"]));
            message.To.Add(MailboxAddress.Parse(to));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = body };

            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
