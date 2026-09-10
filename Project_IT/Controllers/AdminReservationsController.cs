using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using log4net;
using Project_IT.Models;

namespace Project_IT.Controllers
{
    [Authorize]
    public class AdminReservationsController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminReservationsController));
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            try
            {
                var reservations = db.Reservations;
                if (reservations != null && reservations.Any()) {
                    var result = reservations.OrderByDescending(r => r.CreatedOn).ToList();
                    return View(result);
                }
                return View(new List<Reservation>());
            }
            catch (Exception ex)
            {
                log.Error("Error in Index: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Create()
        {
            try
            {
                return View(new Reservation { CreatedOn = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                log.Error("Error in Create: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: FeedItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reservation reservation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (reservation.CreatedOn == default(DateTime))
                    {
                        reservation.CreatedOn = DateTime.UtcNow;
                    }
                    db.Reservations.Add(reservation);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(reservation);
            }
            catch (Exception ex)
            {
                log.Error("Error in Create [POST]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var reservation = db.Reservations.Find(id);
                if (reservation == null)
                {
                    return HttpNotFound();
                }

                return View(reservation);
            }
            catch (Exception ex)
            {
                log.Error("Error in Details: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Edit(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var reservation = db.Reservations.Find(id);
                if (reservation == null)
                {
                    return HttpNotFound();
                }

                return View(reservation);
            }
            catch (Exception ex)
            {
                log.Error("Error in Edit: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Reservation reservation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(reservation);
                }

                var existing = db.Reservations.Find(reservation.Id);
                if (existing == null)
                {
                    return HttpNotFound();
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

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Error in Edit [POST]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Delete(int? id)
        {
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                var reservation = db.Reservations.Find(id);
                if (reservation == null)
                {
                    return HttpNotFound();
                }

                return View(reservation);
            }
            catch (Exception ex)
            {
                log.Error("Error in Delete: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                var reservation = db.Reservations.Find(id);
                if (reservation == null)
                {
                    return HttpNotFound();
                }

                db.Reservations.Remove(reservation);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Error in DeleteConfirmed: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
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
