using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Project_IT.Models;
using Project_IT.Models.ViewModels;
using Project_IT.Services.Interfaces;

namespace Project_IT.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HomeController));
        private readonly ISettingService _settingService;

        public HomeController(ISettingService settingService)
        {
            _settingService = settingService;
        }

        public HomeController()
        {
        }

        public ActionResult Index()
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                log.Error("Error in Index: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        public async Task<ActionResult> About()
        {
            try
            {
                ViewBag.Message = "Your application description page.";

                if (_settingService != null)
                {
                    var prefs = await _settingService.GetSettingAsync<GeneralAppPreferencesViewModel>("GeneralAppPreferences");
                    if (prefs != null && !string.IsNullOrWhiteSpace(prefs.AboutUsGalleryPath))
                    {
                        string relativePath = prefs.AboutUsGalleryPath.Trim();
                        if (!relativePath.StartsWith("/"))
                        {
                            relativePath = "/" + relativePath;
                        }

                        string physicalPath = Server.MapPath(relativePath);
                        if (!string.IsNullOrEmpty(physicalPath) && Directory.Exists(physicalPath))
                        {
                            var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".webm" };
                            var videoExtensions = new[] { ".mp4", ".webm" };

                            var files = Directory.GetFiles(physicalPath)
                                .Where(f => validExtensions.Contains(Path.GetExtension(f).ToLower()))
                                .OrderBy(f => f);

                            var mediaList = new List<MediaItem>();
                            foreach (var file in files)
                            {
                                string fileName = Path.GetFileName(file);
                                string ext = Path.GetExtension(file).ToLower();

                                mediaList.Add(new MediaItem
                                {
                                    Url = $"{relativePath.TrimEnd('/')}/{fileName}",
                                    IsVideo = videoExtensions.Contains(ext)
                                });
                            }

                            if (mediaList.Any())
                            {
                                ViewBag.GalleryMedia = mediaList;
                            }
                        }
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                log.Error("Error in About: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Contact()
        {
            try
            {
                ViewBag.Message = "Your contact page.";

                return View();
            }
            catch (Exception ex)
            {
                log.Error("Error in Contact: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Error()
        {
            try
            {
                return View();
            }
            catch
            {
            }
            return View();
        }
    }
}
