using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MailHelper;

namespace Vitinerario.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlMessage)
        {
            var host = _configuration["MailSettings:Host"];
            var port = int.Parse(_configuration["MailSettings:Port"]);
            var fromEmail = _configuration["MailSettings:Mail"];
            var fromEmailPwd = _configuration["MailSettings:Password"];
            var senderName = _configuration["MailSettings:SenderName"];
            var enableSSL = bool.Parse(_configuration["MailSettings:EnableSSL"]);

            // HelperMailKit does not have a parameterless constructor.
            // Using the constructor: HelperMailKit(string fromEmail, string fromEmailPwd, string host, int port, bool enableSSL, string senderName)
            var helper = new HelperMailKit(fromEmail, fromEmailPwd, host, port, enableSSL, senderName);

            await helper.SendEmailHtmlAsync(
                toEmail,
                subject,
                htmlMessage,
                new List<InlineImage>(),
                new List<AttachmentFile>(),
                null
            );
        }
    }
}
