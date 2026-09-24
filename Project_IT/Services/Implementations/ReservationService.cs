using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using log4net;
using Project_IT.Models;
using Project_IT.Services.Interfaces;

namespace Project_IT.Services.Implementations
{
    public class ReservationService : IReservationService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReservationService));
        private readonly ApplicationDbContext _db;

        public ReservationService(ApplicationDbContext db)
        {
            _db = db;
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

        public bool ProcessReservationSubmission(ReservationSubmissionModel model, string userAgent, string userHostAddress, out string errorMessage, out Dictionary<string, IEnumerable<string>> validationErrors)
        {
            errorMessage = null;
            validationErrors = null;

            try
            {
                if (model == null)
                {
                    errorMessage = "Invalid submission model.";
                    return false;
                }

                var email = (model.Email ?? string.Empty).Trim();
                var fullName = (model.FullName ?? string.Empty).Trim();
                var phone = (model.Phone ?? string.Empty).Trim();

                if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(phone))
                {
                    errorMessage = "Missing required fields.";
                    return false;
                }

                var checkIn = ParseDate(model.CheckInDate) ?? DateTime.UtcNow.Date;
                var checkOut = ParseDate(model.CheckOutDate) ?? checkIn;
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
                    Days = CalculateDays(checkIn, checkOut),
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
                    validationErrors = results
                        .GroupBy(r => r.MemberNames.FirstOrDefault() ?? string.Empty)
                        .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage));
                    errorMessage = "Validation failed.";
                    return false;
                }

                _db.Reservations.Add(reservation);
                _db.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                log.Error("Error processing reservation submission: " + ex.Message, ex);
                throw;
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
