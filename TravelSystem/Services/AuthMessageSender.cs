using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TravelSystem.Common;

namespace TravelSystem.Services
{
    public class AuthMessageSender : IEmailSender
    {
        private readonly AppSettings _appSettings;

        public AuthMessageSender(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            // Plug in your email service here to send an email.
            return ConfigSmtpAsync(email, subject, message);
            //return Task.FromResult(0);
        }
        private async Task ConfigSmtpAsync(string email, string subject, string message)
        {
            var smtpServerAddress = _appSettings.SmtpServer;
            var smtpPort = _appSettings.SmtpPort;
            var smtpUserName = _appSettings.SmtpUserName;
            var smtpPassword = _appSettings.SmtpPassword;
            var fromEmailAddress = _appSettings.FromEmailAddress;

            var emailMessage = new MailMessage
            {
                From = new MailAddress(fromEmailAddress),
                Subject = subject,
                Body = message,
                IsBodyHtml = true,
                SubjectEncoding = Encoding.GetEncoding("iso-8859-1"),
                BodyEncoding = Encoding.GetEncoding("iso-8859-1"),
                HeadersEncoding = Encoding.GetEncoding("iso-8859-1")
            };
            emailMessage.To.Add(new MailAddress(email));

            var smtpServer = new SmtpClient(smtpServerAddress, int.Parse(smtpPort)) { EnableSsl = false };
            if (!string.IsNullOrWhiteSpace(smtpUserName) || !string.IsNullOrWhiteSpace(smtpPassword))
                smtpServer.Credentials = new System.Net.NetworkCredential(smtpUserName, smtpPassword);
            await smtpServer.SendMailAsync(emailMessage);
        }
    }
}
