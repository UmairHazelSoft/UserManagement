using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using UserManagementSystem.Helpers;

namespace UserManagementSystem.Sender.Email
{
    public class EmailSender : IEmailSender
    {
        private IConfiguration _configuration;
        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            
            string smtpHost = _configuration[AppSettings.SmtpHost] ;
            int smtpPort =    int.Parse(_configuration[AppSettings.SmtpPort] ?? "587"); ;
            string smtpUser = _configuration[AppSettings.SmtpUser];
            string smtpPass = _configuration[AppSettings.SmtpPass];
            string fromEmail = _configuration[AppSettings.FromEmail];

        using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPass),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await client.SendMailAsync(mailMessage);

        }
    }
}
