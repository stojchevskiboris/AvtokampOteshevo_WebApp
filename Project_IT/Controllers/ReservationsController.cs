using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using Project_IT.Models;
using Project_IT.Models.ViewModels;
using Project_IT.Services;
using Project_IT.Services.Interfaces;

namespace Project_IT.Controllers
{
    public class ReservationsController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReservationsController));
        private readonly IReservationService _reservationService;
        private readonly ISettingService _settingService;
        private readonly IResendEmailService _emailService;

        public ReservationsController(IReservationService reservationService, ISettingService settingService, IResendEmailService emailService)
        {
            _reservationService = reservationService;
            _settingService = settingService;
            _emailService = emailService;
        }

        public async Task<ActionResult> New()
        {
            try
            {
                var prices = await _settingService.GetSettingAsync("BookingPrices", new BookingPricesViewModel
                {
                    PricePerNightBungalow1 = 10.00m,
                    LabelBungalow1 = new TItleModel { Name = "Бунгалов 1", NameEn = "Bungalow 1" },
                    PricePerNightBungalow2 = 12.00m,
                    LabelBungalow2 = new TItleModel { Name = "Бунгалов 2", NameEn = "Bungalow 2" },
                    PricePerNightBungalow3 = 15.00m,
                    LabelBungalow3 = new TItleModel { Name = "Бунгалов 3", NameEn = "Bungalow 3" },
                    PricePerNightTrailer = 8.00m,
                    LabelTrailer = new TItleModel { Name = "Приколка", NameEn = "Trailer" },
                    PitchFee = 5.00m,
                    LabelPitch = new TItleModel { Name = "Такса за плац", NameEn = "Pitch Fee" }
                });

                ViewBag.Prices = prices;
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Error in New: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Save(ReservationSubmissionModel model)
        {
            try
            {
                if (model == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var userAgent = Request?.UserAgent;
                var userHostAddress = GetRequestIpAddress();

                bool success = _reservationService.ProcessReservationSubmission(
                    model, userAgent, userHostAddress, out var errorMessage, out var validationErrors);

                if (success)
                {
                    // Email Dispatch Workflow
                    try
                    {
                        // Extract client culture/locale
                        string requestedCulture = Request?.Headers["x-user-locale"]
                            ?? Request?.Headers["culture"]
                            ?? model.UserCountry;

                        // 1. Admin Notification Email (Always Macedonian)
                        string adminEmailAddress = ConfigurationManager.AppSettings["MailFromAddress"];
                        if (!string.IsNullOrWhiteSpace(adminEmailAddress))
                        {
                            string adminSubject = $"Нова Резервација - {model.FullName}";
                            string adminBody = EmailTemplateBuilder.BuildAdminNotificationEmail(model);
                            await _emailService.SendEmailAsync(adminEmailAddress, adminSubject, adminBody);
                        }

                        // 2. Client Confirmation Email (Localized)
                        if (!string.IsNullOrWhiteSpace(model.Email))
                        {
                            string clientSubject = EmailTemplateBuilder.GetSubjectForCulture(requestedCulture);
                            string clientBody = EmailTemplateBuilder.BuildClientConfirmationEmail(model, requestedCulture);
                            await _emailService.SendEmailAsync(model.Email, clientSubject, clientBody);
                        }
                    }
                    catch (Exception emailEx)
                    {
                        log.Error("Failed sending email notifications for reservation submission: " + emailEx.Message, emailEx);
                        // Do not fail the submission if email sending encounters an issue, but log it.
                    }

                    return Json(new { status = "success" });
                }

                if (validationErrors != null)
                {
                    return Json(new { status = "error", errors = validationErrors });
                }

                return Json(new { status = "error", message = errorMessage ?? "Unable to save reservation." });
            }
            catch (Exception ex)
            {
                log.Error("Error in Save: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        private string GetRequestIpAddress()
        {
            var forwarded = Request?.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (!string.IsNullOrWhiteSpace(forwarded))
            {
                var firstIp = forwarded.Split(',').FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(firstIp))
                {
                    return firstIp.Trim();
                }
            }

            return Request?.UserHostAddress;
        }
    }
}
