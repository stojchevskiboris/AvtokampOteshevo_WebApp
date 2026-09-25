using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using Project_IT.Models;
using Project_IT.Services.Interfaces;

namespace Project_IT.Controllers
{
    [Authorize]
    public class AdminSettingsController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(AdminSettingsController));
        private readonly ISettingService _settingService;

        public AdminSettingsController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        // GET: AdminSettings/BookingPrices
        public async Task<ActionResult> BookingPrices()
        {
            try
            {
                var model = await _settingService.GetSettingAsync("BookingPrices", new BookingPricesViewModel
                {
                    PricePerNightBungalow1 = 10.00m,
                    PricePerNightBungalow2 = 12.00m,
                    PricePerNightBungalow3 = 15.00m,
                    PricePerNightTrailer = 8.00m,
                    PitchFee = 5.00m
                });

                return View(model);
            }
            catch (Exception ex)
            {
                log.Error("Error in BookingPrices [GET]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: AdminSettings/BookingPrices
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> BookingPrices(BookingPricesViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                await _settingService.SaveSettingAsync("BookingPrices", model, "Configures pitch and per-person reservation rates.");
                TempData["SuccessMessage"] = "Цените за резервација се успешно ажурирани!";

                return RedirectToAction("BookingPrices");
            }
            catch (Exception ex)
            {
                log.Error("Error in BookingPrices [POST]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
