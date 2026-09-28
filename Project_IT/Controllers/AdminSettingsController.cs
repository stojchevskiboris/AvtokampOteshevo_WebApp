using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using log4net;
using Project_IT.Models;
using Project_IT.Models.ViewModels;
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

        // GET: AdminSettings
        public ActionResult Index()
        {
            try
            {
                var model = GetSettingsDashboardModel();
                return View(model);
            }
            catch (Exception ex)
            {
                log.Error("Error in AdminSettings Index [GET]: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        private SettingsDashboardViewModel GetSettingsDashboardModel()
        {
            return new SettingsDashboardViewModel
            {
                Title = "Табла со Подесувања",
                Description = "Централизирана платформа за управување со сите конфигурациски модули и опции на апликацијата.",
                Modules = new System.Collections.Generic.List<SettingsModuleViewModel>
                {
                    new SettingsModuleViewModel
                    {
                        Key = "BookingPrices",
                        Title = "Цени за Резервација",
                        Description = "Конфигурација на ценовникот за сите типови сместување (бунгалови, приколки) и дополнителни такси.",
                        Category = "Резервации и Ценовник",
                        ActionName = "BookingPrices",
                        ControllerName = "AdminSettings",
                        IsEnabled = true,
                        BadgeText = "Активно",
                        BadgeClass = "bg-emerald-50 text-emerald-700 border-emerald-200"
                    },
                    new SettingsModuleViewModel
                    {
                        Key = "EmailSettings",
                        Title = "Подесувања за Е-пошта",
                        Description = "Управување со шаблони, известувања за нови резервации и е-пошта извештаи.",
                        Category = "Систем и Известувања",
                        ActionName = "EmailSettings",
                        ControllerName = "AdminSettings",
                        IsEnabled = false,
                        BadgeText = "Наскоро",
                        BadgeClass = "bg-slate-100 text-slate-600 border-slate-200"
                    },
                    new SettingsModuleViewModel
                    {
                        Key = "PaymentGatewayIntegrations",
                        Title = "Интеграција за Плаќање",
                        Description = "Конфигурација на платежни картички, гишеа и процесори за онлајн плаќања.",
                        Category = "Плаќања и Финансии",
                        ActionName = "PaymentGateway",
                        ControllerName = "AdminSettings",
                        IsEnabled = false,
                        BadgeText = "Наскоро",
                        BadgeClass = "bg-slate-100 text-slate-600 border-slate-200"
                    },
                    new SettingsModuleViewModel
                    {
                        Key = "GeneralAppPreferences",
                        Title = "Општи Опции на Апликацијата",
                        Description = "Конфигурација на контакт информации, работни часови, социјални мрежи и основни поставки.",
                        Category = "Општо",
                        ActionName = "GeneralPreferences",
                        ControllerName = "AdminSettings",
                        IsEnabled = false,
                        BadgeText = "Наскоро",
                        BadgeClass = "bg-slate-100 text-slate-600 border-slate-200"
                    }
                }
            };
        }

        // GET: AdminSettings/BookingPrices
        public async Task<ActionResult> BookingPrices()
        {
            try
            {
                var model = await _settingService.GetSettingAsync("BookingPrices", new BookingPricesViewModel
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
