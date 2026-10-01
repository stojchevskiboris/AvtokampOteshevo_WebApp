using log4net;
using Newtonsoft.Json;
using Project_IT.Models;
using Project_IT.Models.ViewModels;
using Project_IT.Services.Interfaces;
using System;
using System.Configuration;
using System.Data.Entity;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;

namespace Project_IT.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private static readonly HttpClient _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.resend.com/")
        };

        private ILog log = LogManager.GetLogger(typeof(EmailService));

        private readonly string _apiKey;
        private readonly string _fromAddress;

        public EmailService()
        {
            _apiKey = ConfigurationManager.AppSettings["ResendApiKey"];
            _fromAddress = ConfigurationManager.AppSettings["MailFromAddress"];
        }

        public async Task SendEmailAsync(string to, string subject, string htmlContent)
        {
            try
            {

                var payload = new SendEmailRequestModel
                {
                    From = _fromAddress,
                    To = new[] { to },
                    Subject = subject,
                    Html = htmlContent
                };

                var json = JsonConvert.SerializeObject(payload);

                using (var request = new HttpRequestMessage(HttpMethod.Post, "emails"))
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
                    request.Content = new StringContent(json, Encoding.UTF8, "application/json");

                    var response = await _httpClient.SendAsync(request);
                    var responseContent = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Failed to send email via Resend API: {response.StatusCode} - {responseContent}");
                    }

                    var result = JsonConvert.DeserializeObject(responseContent);
                    return;
                }
            }
            catch (Exception ex)
            {
                log.Error("An error occurred while sending email via Resend API.", ex);
                throw new Exception("An error occurred while sending email via Resend API.", ex);
            }
        }
    }
}