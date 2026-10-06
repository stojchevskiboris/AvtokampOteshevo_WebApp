using log4net;
using Newtonsoft.Json;
using Project_IT.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Project_IT.Services.Implementations
{
    public class EmailJsService : IEmailJsService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(EmailJsService));
        private static readonly HttpClient httpClient = new HttpClient();

        private const string EmailJsApiUrl = "https://api.emailjs.com/api/v1.0/email/send";
        private const string ServiceId = "default_service";
        private const string TemplateId = "template_bjhyjh8";
        private const string UserId = "0YVQaG2ITtZEM__Mt";

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlContent, Dictionary<string, string> customParams = null)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(toEmail))
                {
                    log.Warn("SendEmailAsync called with empty recipient email address.");
                    return false;
                }

                var templateParams = new Dictionary<string, object>
                {
                    { "to_email", toEmail },
                    { "customer_email", toEmail },
                    { "subject", subject ?? string.Empty },
                    { "message", htmlContent ?? string.Empty },
                    { "html_content", htmlContent ?? string.Empty },
                    { "reply_to", "avtokamp.otesevo@gmail.com" },
                    { "from_name", "Autocamp Otesevo" }
                };

                if (customParams != null)
                {
                    foreach (var kvp in customParams)
                    {
                        templateParams[kvp.Key] = kvp.Value;
                    }
                }

                var payload = new
                {
                    service_id = ServiceId,
                    template_id = TemplateId,
                    user_id = UserId,
                    template_params = templateParams
                };

                var json = JsonConvert.SerializeObject(payload);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var request = new HttpRequestMessage(HttpMethod.Post, EmailJsApiUrl)
                {
                    Content = content
                };

                request.Headers.Add("Origin", "https://avtokampoteshevo.com");

                var response = await httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    log.Info($"Email successfully sent via EmailJS to {toEmail} with subject '{subject}'.");
                    return true;
                }

                var respBody = await response.Content.ReadAsStringAsync();
                log.Error($"EmailJS API call failed with status code {response.StatusCode} for recipient {toEmail}: {respBody}");
                return false;
            }
            catch (Exception ex)
            {
                log.Error($"Error sending email via EmailJS to {toEmail}: " + ex.Message, ex);
                return false;
            }
        }
    }
}
