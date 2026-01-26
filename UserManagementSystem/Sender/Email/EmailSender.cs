using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using UserManagementSystem.Helpers;

namespace UserManagementSystem.Sender.Email
{
    public class EmailSender : IEmailSender
    {

        public EmailSender()
        {
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Get SMTP settings from appsettings.json
            string smtpHost = AppSettings.SmtpHost ;
            int smtpPort = AppSettings.SmtpPort;
            string smtpUser = AppSettings.SmtpUser;
            string smtpPass = AppSettings.SmtpPass;
            string fromEmail = AppSettings.FromEmail;

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
