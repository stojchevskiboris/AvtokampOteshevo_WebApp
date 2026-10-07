using log4net;
using Project_IT.Services.Interfaces;
using System;
using System.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Project_IT.Services.Implementations
{
    public class SmtpService : ISmtpService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SmtpService));

        private readonly string _smtpServer = ConfigurationManager.AppSettings["SmtpServer"];
        private readonly int _smtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"] ?? "587");
        private readonly string _smtpUsername = ConfigurationManager.AppSettings["SmtpUsername"];
        private readonly string _smtpPassword = ConfigurationManager.AppSettings["SmtpPassword"];
        private readonly string _fromEmail = ConfigurationManager.AppSettings["FromEmail"];
        private readonly string _replyToEmail = ConfigurationManager.AppSettings["ReplyToEmail"];
        private readonly string _fromName = ConfigurationManager.AppSettings["FromName"];

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string replyToEmail = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmail))
                {
                    log.Warn("SendEmailAsync called with empty recipient email address.");
                    return false;
                }

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(_fromEmail, _fromName);
                    mailMessage.To.Add(new MailAddress(toEmail));
                    mailMessage.Subject = subject ?? string.Empty;
                    mailMessage.Body = htmlContent ?? string.Empty;
                    mailMessage.IsBodyHtml = true;

                    if (replyToEmail != null && !string.IsNullOrWhiteSpace(replyToEmail))
                    {
                        mailMessage.ReplyToList.Add(new MailAddress(replyToEmail));
                    }
                    else if (!string.IsNullOrWhiteSpace(_replyToEmail))
                    {
                        mailMessage.ReplyToList.Add(new MailAddress(_replyToEmail));
                    }

                    using (var smtpClient = new SmtpClient(_smtpServer, _smtpPort))
                    {
                        smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        smtpClient.EnableSsl = true;

                        await smtpClient.SendMailAsync(mailMessage);
                    }
                }

                log.Info($"Email successfully sent via Google SMTP to {toEmail} with subject '{subject}'.");
                return true;
            }
            catch (Exception ex)
            {
                log.Error($"Error sending email via Google SMTP to {toEmail}: " + ex.Message, ex);
                return false;
            }
        }
    }
}