using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using Vitinerario.Models;
using Vitinerario.Models.Settings;
using MailHelper;
using Microsoft.Extensions.Configuration;

namespace Vitinerario.Services
{
    public class EmailService : IEmailService
    {
        private readonly MailSettings _mailSettings;
        private readonly string _adminEmail;

        public EmailService(IOptions<MailSettings> mailSettingsOptions, IConfiguration configuration)
        {
            _mailSettings = mailSettingsOptions.Value;
            _adminEmail = configuration.GetValue<string>("AdminEmail") ?? "info@vitinerario.it";
        }

        public async Task<bool> SendContactEmailAsync(ContactFormViewModel model)
        {
            try
            {
                var mailHelper = new HelperMailKit(
                    _mailSettings.Mail,
                    _mailSettings.Password,
                    _mailSettings.Host,
                    _mailSettings.Port,
                    _mailSettings.EnableSSL,
                    _mailSettings.SenderName
                );

                string subject = $"Nuova richiesta di contatto da {model.FullName}";

                string htmlBody = $@"
                    <h2>Nuovo messaggio dal modulo di contatto Vitinerario</h2>
                    <p><strong>Nome:</strong> {model.FullName}</p>
                    <p><strong>Email:</strong> {model.Email}</p>
                    <p><strong>Messaggio:</strong></p>
                    <p>{model.Message}</p>
                ";

                // Sending to the app's admin info email
                await mailHelper.SendEmailHtmlAsync(
                    toEmail: _adminEmail,
                    subject: subject,
                    htmlBody: htmlBody,
                    inlineImages: null,
                    attachments: null,
                    plainTextAlternative: model.Message
                );

                return true;
            }
            catch (System.Exception ex)
            {
                // In a real application, we would log the exception here
                // _logger.LogError(ex, "Error sending contact email");
                System.Console.WriteLine($"Error sending email: {ex.Message}");
                return false;
            }
        }
    }
}
