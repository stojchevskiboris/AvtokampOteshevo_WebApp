using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Project_IT.Models;

namespace Project_IT.Controllers
{
    [Authorize]
    public class AdminReservationsController : Controller
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            var reservations = db.Reservations.OrderByDescending(r => r.CreatedOn).ToList();
            return View(reservations);
        }

        public ActionResult Details(int? id)
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

        public ActionResult Edit(int? id)
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,FullName,Denovi,Lica,Cena,Telefon,DataNaPristignuvanje,DataNaZaminuvanje,Smestuvanje,Status,Info")] Reservation reservation)
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
            existing.Telefon = reservation.Telefon;
            existing.Smestuvanje = reservation.Smestuvanje;
            existing.DataNaPristignuvanje = reservation.DataNaPristignuvanje;
            existing.DataNaZaminuvanje = reservation.DataNaZaminuvanje;
            existing.Lica = reservation.Lica;
            existing.Denovi = reservation.Denovi;
            existing.Status = reservation.Status;
            existing.Info = reservation.Info;
            existing.Cena = reservation.Cena;
            existing.ModifiedOn = DateTime.UtcNow;

            if (!string.IsNullOrWhiteSpace(existing.FullName))
            {
                var parts = existing.FullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                existing.Ime = parts.Length > 0 ? parts[0] : existing.FullName;
                existing.Prezime = parts.Length > 1 ? string.Join(" ", parts.Skip(1)) : string.Empty;
            }

            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int? id)
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

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
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
