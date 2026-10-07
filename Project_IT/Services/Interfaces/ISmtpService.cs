using System.Threading.Tasks;

namespace Project_IT.Services.Interfaces
{
    public interface ISmtpService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, string replyToEmail = null);
    }
}
