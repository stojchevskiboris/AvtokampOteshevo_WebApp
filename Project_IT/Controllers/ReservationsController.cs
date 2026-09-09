using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Project_IT.Models;

namespace Project_IT.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Save(ReservationSubmissionModel model)
        {
            if (model == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var email = (model.Email ?? string.Empty).Trim();
            var fullName = (model.FullName ?? string.Empty).Trim();
            var phone = (model.Phone ?? string.Empty).Trim();

            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
            {
                return Json(new { status = "error", message = "Missing required fields." });
            }

            var checkIn = ParseDate(model.CheckInDate) ?? DateTime.UtcNow.Date;
            var checkOut = ParseDate(model.CheckOutDate) ?? checkIn;
            var guests = ParseInt(model.Guests);

            var nameParts = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            var firstName = nameParts.Length > 0 ? nameParts[0] : fullName;
            var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : string.Empty;

            var userAgent = Request?.UserAgent;
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
                Days = CalculateDays(checkIn, checkOut),
                Info = model.Info,
                Status = "New",
                CreatedOn = DateTime.UtcNow,
                ModifiedOn = null,
                UserOs = model.UserOs,
                UserPlatform = model.UserPlatform,
                UserAgent = string.IsNullOrWhiteSpace(model.UserAgent) ? userAgent : model.UserAgent,
                IPAddress = GetRequestIpAddress(),
                UserIp = string.IsNullOrWhiteSpace(model.UserIp) ? GetRequestIpAddress() : model.UserIp,
                UserBrowser = model.UserBrowser,
                UserVersion = model.UserVersion,
                UserCountry = model.UserCountry,
                UserReferrer = model.UserReferrer,
                Price = 0
            };

            if (!TryValidateModel(reservation))
            {
                return Json(new
                {
                    status = "error",
                    errors = ModelState.Where(kv => kv.Value.Errors.Any())
                                       .ToDictionary(kv => kv.Key, kv => kv.Value.Errors.Select(e => e.ErrorMessage))
                });
            }

            db.Reservations.Add(reservation);
            db.SaveChanges();

            return Json(new { status = "success" });
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
