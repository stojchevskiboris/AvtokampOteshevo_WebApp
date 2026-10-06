using System.Collections.Generic;
using System.Threading.Tasks;

namespace Project_IT.Services.Interfaces
{
    public interface IEmailJsService
    {
        Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, Dictionary<string, string> customParams = null);
    }
}
