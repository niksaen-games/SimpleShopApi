using MailKit.Net.Smtp;
using MimeKit;
using System.Reflection;
using System.Text;

namespace SimpleShopApi.Services
{
    public class MailService(IConfiguration configuration)
    {
        public async Task<bool> SendResetPasswordCodeAsync(string email, string name, string surname, string code)
        {

            try
            {
                string templatePath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
                    "Assets",
                    "MailTemplates",
                    "ResetPasswordTemplate.html"
                );

                if (!File.Exists(templatePath))
                {
                    var assemblyDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
                    templatePath = Path.Combine(assemblyDir ?? "", "Assets", "MailTemplates", "ResetPasswordTemplate.html");
                }

                string templateContent = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);

                string emailBody = templateContent
                    .Replace("{{name}}", name ?? "")
                    .Replace("{{surname}}", surname ?? "")
                    .Replace("{{resetCode}}", code ?? "");

                await SendHtmlEmailAsync(email, "Reset password", emailBody);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке письма: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendConfirmEmailCodeAsync(string email, string name, string surname, string code)
        {

            try
            {
                string templatePath = Path.Combine(
                    Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "",
                    "Assets",
                    "MailTemplates",
                    "ConfirmEmailTemplate.html"
                );

                if (!File.Exists(templatePath))
                {
                    var assemblyDir = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
                    templatePath = Path.Combine(assemblyDir ?? "", "Assets", "MailTemplates", "ConfirmEmailTemplate.html");
                }

                string templateContent = await File.ReadAllTextAsync(templatePath, Encoding.UTF8);

                string emailBody = templateContent
                    .Replace("{{name}}", name ?? "")
                    .Replace("{{surname}}", surname ?? "")
                    .Replace("{{resetCode}}", code ?? "");

                await SendHtmlEmailAsync(email, "Confirm email", emailBody);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при отправке письма: {ex.Message}");
                return false;
            }
        }
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
