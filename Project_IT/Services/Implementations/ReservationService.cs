using log4net;
using Project_IT.Helpers;
using Project_IT.Models;
using Project_IT.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_IT.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReservationService));
        private readonly ApplicationDbContext _db;
        private readonly IEmailJsService _emailJsService;

        public ReservationService(ApplicationDbContext db, IEmailJsService emailJsService)
        {
            _db = db;
            _emailJsService = emailJsService;
        }

        public IEnumerable<Reservation> GetAllReservations()
        {
            try
            {
                var reservations = _db.Reservations;
                if (reservations != null && reservations.Any())
                {
                    return reservations.OrderByDescending(r => r.CreatedOn).ToList();
                }
                return new List<Reservation>();
            }
            catch (Exception ex)
            {
                log.Error("Error fetching all reservations: " + ex.Message, ex);
                throw;
            }
        }

        public Reservation GetReservationById(int id)
        {
            try
            {
                return _db.Reservations.Find(id);
            }
            catch (Exception ex)
            {
                log.Error($"Error fetching reservation with ID {id}: " + ex.Message, ex);
                throw;
            }
        }

        public void CreateReservation(Reservation reservation)
        {
            try
            {
                if (reservation.CreatedOn == default(DateTime))
                {
                    reservation.CreatedOn = DateTime.UtcNow;
                }
                _db.Reservations.Add(reservation);
                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error("Error creating reservation: " + ex.Message, ex);
                throw;
            }
        }

        public void UpdateReservation(Reservation reservation)
        {
            try
            {
                var existing = _db.Reservations.Find(reservation.Id);
                if (existing == null)
                {
                    throw new KeyNotFoundException($"Reservation with ID {reservation.Id} was not found.");
                }

                existing.FullName = reservation.FullName;
                existing.Email = reservation.Email;
                existing.Phone = reservation.Phone;
                existing.AccommodationType = reservation.AccommodationType;
                existing.CheckInDate = reservation.CheckInDate;
                existing.CheckOutDate = reservation.CheckOutDate;
                existing.Guests = reservation.Guests;
                existing.Days = reservation.Days;
                existing.Status = reservation.Status;
                existing.Info = reservation.Info;
                existing.Price = reservation.Price;
                existing.ModifiedOn = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(existing.FullName))
                {
                    var parts = existing.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    existing.FirstName = parts.Length > 0 ? parts[0] : existing.FullName;
                    existing.LastName = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
                }

                _db.SaveChanges();
            }
            catch (Exception ex)
            {
                log.Error($"Error updating reservation with ID {reservation?.Id}: " + ex.Message, ex);
                throw;
            }
        }

        public void DeleteReservation(int id)
        {
            try
            {
                var reservation = _db.Reservations.Find(id);
                if (reservation != null)
                {
                    _db.Reservations.Remove(reservation);
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error deleting reservation with ID {id}: " + ex.Message, ex);
                throw;
            }
        }

        public async Task<ReservationSubmissionResult> ProcessReservationSubmissionAsync(
            ReservationSubmissionModel model, string userAgent, string userHostAddress, string culture = null)
        {
            var result = new ReservationSubmissionResult();

            try
            {
                if (model == null)
                {
                    result.ErrorMessage = "Invalid submission model.";
                    return result;
                }

                var email = (model.Email ?? string.Empty).Trim();
                var fullName = (model.FullName ?? string.Empty).Trim();
                var phone = (model.Phone ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
                {
                    result.ErrorMessage = "Missing required fields.";
                    return result;
                }

                var checkIn = string.IsNullOrEmpty(model.CheckInDate) ? null : ParseDate(model.CheckInDate);
                var checkOut = string.IsNullOrEmpty(model.CheckOutDate) ? null : ParseDate(model.CheckOutDate);
                var guests = ParseInt(model.Guests);

                var nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var firstName = nameParts.Length > 0 ? nameParts[0] : fullName;
                var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

                var ipAddress = !string.IsNullOrWhiteSpace(userHostAddress) ? userHostAddress : null;

                var reservation = new Reservation
                {
                    FullName = fullName,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    Phone = phone,
                    Guests = guests < 0 ? 0 : guests,
                    AccommodationType = model.AccommodationType,
                    CheckInDate = checkIn,
                    CheckOutDate = checkOut,
                    Days = checkIn.HasValue && checkOut.HasValue ? CalculateDays(checkIn.Value, checkOut.Value) : 0,
                    Info = model.Info,
                    Status = "New",
                    CreatedOn = DateTime.UtcNow,
                    ModifiedOn = null,
                    UserOs = model.UserOs,
                    UserPlatform = model.UserPlatform,
                    UserAgent = string.IsNullOrWhiteSpace(model.UserAgent) ? userAgent : model.UserAgent,
                    IPAddress = ipAddress,
                    UserIp = string.IsNullOrWhiteSpace(model.UserIp) ? ipAddress : model.UserIp,
                    UserBrowser = model.UserBrowser,
                    UserVersion = model.UserVersion,
                    UserCountry = model.UserCountry,
                    UserReferrer = model.UserReferrer,
                    Price = 0
                };

                var validationContext = new ValidationContext(reservation, serviceProvider: null, items: null);
                var results = new List<ValidationResult>();

                if (!Validator.TryValidateObject(reservation, validationContext, results, validateAllProperties: true))
                {
                    result.ValidationErrors = results
                        .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
                        .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage));
                    result.ErrorMessage = "Validation failed.";
                    return result;
                }

                _db.Reservations.Add(reservation);
                _db.SaveChanges();

                var activeCulture = !string.IsNullOrWhiteSpace(culture) ? culture : CultureHelper.Current;

                await SendAdminNotificationEmailAsync(reservation);
                await SendClientConfirmationEmailAsync(reservation, activeCulture);

                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                log.Error("Error processing reservation submission: " + ex.Message, ex);
                throw;
            }
        }

        private async Task<bool> SendAdminNotificationEmailAsync(Reservation reservation)
        {
            try
            {
                string adminEmail = "avtokamp.otesevo@gmail.com";
                string templateContent = LoadTemplate("adminTemplate.txt");

                if (string.IsNullOrWhiteSpace(templateContent))
                {
                    log.Warn("Admin email template (adminTemplate.txt) is empty or could not be loaded.");
                    return false;
                }

                string htmlContent = templateContent
                    .Replace("{{customer_name}}", reservation.FullName ?? string.Empty)
                    .Replace("{{smestuvanje}}", reservation.AccommodationType ?? string.Empty)
                    .Replace("{{guests}}", reservation.Guests.ToString())
                    .Replace("{{check_in}}", reservation.CheckInDate.HasValue ? reservation.CheckInDate.Value.ToString("dd.MM.yyyy") : "/")
                    .Replace("{{check_out}}", reservation.CheckOutDate.HasValue ? reservation.CheckOutDate.Value.ToString("dd.MM.yyyy") : "/")
                    .Replace("{{phone}}", reservation.Phone ?? string.Empty);

                string subject = "Ново барање за резервација - " + reservation.FullName;

                var customParams = new Dictionary<string, string>
                {
                    { "customer_name", reservation.FullName ?? string.Empty },
                    { "smestuvanje", reservation.AccommodationType ?? string.Empty },
                    { "guests", reservation.Guests.ToString() },
                    { "check_in", reservation.CheckInDate.HasValue ? reservation.CheckInDate.Value.ToString("dd.MM.yyyy") : "/" },
                    { "check_out", reservation.CheckOutDate.HasValue ? reservation.CheckOutDate.Value.ToString("dd.MM.yyyy") : "/" },
                    { "phone", reservation.Phone ?? string.Empty }
                };

                return await _emailJsService.SendEmailAsync(adminEmail, subject, htmlContent, customParams);
            }
            catch (Exception ex)
            {
                log.Error("Error in SendAdminNotificationEmailAsync: " + ex.Message, ex);
                return false;
            }
        }

        private async Task<bool> SendClientConfirmationEmailAsync(Reservation reservation, string culture)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(reservation.Email))
                {
                    log.Warn("Client email is empty; skipping client confirmation email.");
                    return false;
                }

                string langCode = CultureHelper.IsSupported(culture) ? culture.ToLowerInvariant() : CultureHelper.Default;
                CultureInfo ci = CultureInfo.GetCultureInfo(langCode == "mk" ? "mk-MK" : (langCode == "en" ? "en-GB" : langCode));

                string templateContent = LoadTemplate("clientTemplate.txt");

                if (string.IsNullOrWhiteSpace(templateContent))
                {
                    log.Warn("Client email template (clientTemplate.txt) is empty or could not be loaded.");
                    return false;
                }

                string title = GetLocalizedString("Email_Client_Title", ci);
                string header = GetLocalizedString("Email_Client_Header", ci);
                string banner = GetLocalizedString("Email_Client_Banner", ci);
                string greeting = GetLocalizedString("Email_Client_Greeting", ci);
                string intro = GetLocalizedString("Email_Client_Intro", ci);
                string summaryTitle = GetLocalizedString("Email_Client_Summary_Title", ci);
                string labelAcc = GetLocalizedString("Res_Accommodation", ci);
                string labelGuests = GetLocalizedString("Res_Guests", ci);
                string labelCheckIn = GetLocalizedString("Res_CheckIn", ci);
                string labelCheckOut = GetLocalizedString("Res_CheckOut", ci);
                string labelPhone = GetLocalizedString("Res_Phone", ci);
                string nextStepsTitle = GetLocalizedString("Email_Client_NextSteps_Title", ci);
                string nextStepsText = GetLocalizedString("Email_Client_NextSteps_Text", ci);
                string notice = GetLocalizedString("Email_Client_Notice", ci);
                string siteName = GetLocalizedString("Site_Name", ci);
                string siteLocation = GetLocalizedString("Site_Footer_Location", ci);

                string htmlContent = templateContent
                    .Replace("{{lang_code}}", langCode)
                    .Replace("{{email_client_title}}", title)
                    .Replace("{{site_name}}", siteName)
                    .Replace("{{email_client_header}}", header)
                    .Replace("{{email_client_banner}}", banner)
                    .Replace("{{email_client_greeting}}", greeting)
                    .Replace("{{customer_name}}", reservation.FullName ?? string.Empty)
                    .Replace("{{email_client_intro}}", intro)
                    .Replace("{{email_client_summary_title}}", summaryTitle)
                    .Replace("{{label_accommodation}}", labelAcc)
                    .Replace("{{smestuvanje}}", reservation.AccommodationType ?? string.Empty)
                    .Replace("{{label_guests}}", labelGuests)
                    .Replace("{{guests}}", reservation.Guests.ToString())
                    .Replace("{{label_check_in}}", labelCheckIn)
                    .Replace("{{check_in}}", reservation.CheckInDate.HasValue ? reservation.CheckInDate.Value.ToString("dd.MM.yyyy") : "/")
                    .Replace("{{label_check_out}}", labelCheckOut)
                    .Replace("{{check_out}}", reservation.CheckOutDate.HasValue ? reservation.CheckOutDate.Value.ToString("dd.MM.yyyy") : "/")
                    .Replace("{{label_phone}}", labelPhone)
                    .Replace("{{phone}}", reservation.Phone ?? string.Empty)
                    .Replace("{{email_client_next_steps_title}}", nextStepsTitle)
                    .Replace("{{email_client_next_steps_text}}", nextStepsText)
                    .Replace("{{email_client_notice}}", notice)
                    .Replace("{{site_location}}", siteLocation);

                string subject = !string.IsNullOrWhiteSpace(title) ? title : "Потврда за примено барање - Автокамп Отешево";

                var customParams = new Dictionary<string, string>
                {
                    { "customer_name", reservation.FullName ?? string.Empty },
                    { "smestuvanje", reservation.AccommodationType ?? string.Empty },
                    { "guests", reservation.Guests.ToString() },
                    { "check_in", reservation.CheckInDate.HasValue ? reservation.CheckInDate.Value.ToString("dd.MM.yyyy") : "/" },
                    { "check_out", reservation.CheckOutDate.HasValue ? reservation.CheckOutDate.Value.ToString("dd.MM.yyyy") : "/" },
                    { "phone", reservation.Phone ?? string.Empty }
                };

                return await _emailJsService.SendEmailAsync(reservation.Email, subject, htmlContent, customParams);
            }
            catch (Exception ex)
            {
                log.Error($"Error in SendClientConfirmationEmailAsync for culture {culture}: " + ex.Message, ex);
                return false;
            }
        }

        private static string LoadTemplate(string templateName)
        {
            try
            {
                string basePath = AppDomain.CurrentDomain.BaseDirectory;
                string path = Path.Combine(basePath, "Templates", templateName);
                if (File.Exists(path))
                {
                    return File.ReadAllText(path, Encoding.UTF8);
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error loading template {templateName}: " + ex.Message, ex);
            }
            return string.Empty;
        }

        private static string GetLocalizedString(string key, CultureInfo ci)
        {
            try
            {
                string str = Otesevo.Resources.Strings.ResourceManager.GetString(key, ci);
                if (!string.IsNullOrEmpty(str))
                {
                    return str;
                }
            }
            catch { }

            try
            {
                return Otesevo.Resources.Strings.ResourceManager.GetString(key, CultureInfo.InvariantCulture)
                    ?? Otesevo.Resources.Strings.ResourceManager.GetString(key)
                    ?? string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }

        private static DateTime? ParseDate(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || string.Equals(value, "Null", StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            if (DateTime.TryParseExact(value, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                return parsed;
            }

            if (DateTime.TryParse(value, out parsed))
            {
                return parsed;
            }

            return null;
        }

        private static int ParseInt(string value)
        {
            return int.TryParse(value, out var parsed) ? parsed : 0;
        }

        private static int CalculateDays(DateTime checkIn, DateTime checkOut)
        {
            var days = (checkOut.Date - checkIn.Date).Days;
            return days > 0 ? days : 0;
        }
    }
}
