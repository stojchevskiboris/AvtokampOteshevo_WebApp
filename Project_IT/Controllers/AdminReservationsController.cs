using System;
using System.Net;
using System.Web.Mvc;
using log4net;
using Project_IT.Models;
using Project_IT.Services.Interfaces;

namespace Project_IT.Controllers
{
    [Authorize]
    public class AdminReservationsController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminReservationsController));
        private readonly IReservationService _reservationService;

        public AdminReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public ActionResult Index()
        {
            try
            {
                var reservations = _reservationService.GetAllReservations();
                return View(reservations);
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Reservation reservation)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _reservationService.CreateReservation(reservation);
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

                var reservation = _reservationService.GetReservationById(id.Value);
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

                var reservation = _reservationService.GetReservationById(id.Value);
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

                _reservationService.UpdateReservation(reservation);
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

                var reservation = _reservationService.GetReservationById(id.Value);
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
                _reservationService.DeleteReservation(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                log.Error("Error in DeleteConfirmed: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
