using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using log4net;
using Project_IT.Models;
using Project_IT.Services.Interfaces;

namespace Project_IT.Controllers
{
    public class ReservationsController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ReservationsController));
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        public ActionResult New()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Error in New: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult Save(ReservationSubmissionModel model)
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
