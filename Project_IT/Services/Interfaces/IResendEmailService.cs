using System.Threading.Tasks;

namespace Project_IT.Services.Interfaces
{
    public interface IResendEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlContent);
    }
}
