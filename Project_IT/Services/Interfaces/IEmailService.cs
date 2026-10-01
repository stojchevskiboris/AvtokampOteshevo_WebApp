using Project_IT.Models.ViewModels;
using System.Threading.Tasks;

namespace Project_IT.Services.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlContent);
    }
}
