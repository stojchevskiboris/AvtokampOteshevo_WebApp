using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using log4net;
using Project_IT.Models;

namespace Project_IT.Controllers
{
    public class ReservationsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ReservationsController));

        // GET: Reservations
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            return View(db.Reservations.ToList());
        }

        // GET: Reservations/Details/5
        [Authorize(Roles = "Admin")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }
        
        // GET: Reservations/Create
        public ActionResult Create()
        {
            return RedirectToAction("New", "Reservations");

            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Email,Ime,Prezime,Denovi,Lica,Cena,Telefon,DataNaPristignuvanje,DataNaZaminuvanje,VremeRezervacija,Info")] Reservation reservation)
        {
            // Redirect to new reservation page
            return RedirectToAction("New", "Reservations");

            if (ModelState.IsValid)
            {
                db.Reservations.Add(reservation);
                db.SaveChanges();
                return RedirectToAction("Final", "Reservations", reservation);
            }

            return View(reservation);
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogReservationData(ReservationSubmissionModel model)
        {
            if (model != null)
            {
                log.Info($"[NEW RESERVATION SUBMISSION] Email: {model.Email} | Name: {model.Ime} | Phone: {model.Telefon} | Guests: {model.Lica} | Accommodation: {model.Smestuvanje} | CheckIn: {model.DataNaPristignuvanje} | CheckOut: {model.DataNaZaminuvanje} | Message: {model.Poraka} | OS: {model.UserOs} | IP: {model.UserIp} | Platform: {model.UserPlatform} | Browser: {model.UserBrowser} | Version: {model.UserVersion} | Country: {model.UserCountry} | Referrer: {model.UserReferrer}");
            }
            return Json(new { status = "success" });
        }

        public ActionResult Final(Reservation reservation)
        {
            return RedirectToAction("New", "Reservations");
            
            return View(reservation);
        }

        public ActionResult Calendar()
        {
            return RedirectToAction("New", "Reservations");

            return View();
        }


        // GET: Reservations/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Email,Ime,Prezime,Denovi,Lica,Cena,Telefon,DataNaPristignuvanje,DataNaZaminuvanje,VremeRezervacija,Info")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                db.Entry(reservation).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Reservation reservation = db.Reservations.Find(id);
            if (reservation == null)
            {
                return HttpNotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Reservation reservation = db.Reservations.Find(id);
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
