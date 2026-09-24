using System;
using System.Web.Mvc;
using log4net;
using Project_IT.Services.Interfaces;

namespace Project_IT.Controllers
{
    public class NewsController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NewsController));
        private readonly INewsService _newsService;

        public NewsController(INewsService newsService)
        {
            _newsService = newsService;
        }

        // GET: News
        public ActionResult Index(int page = 1)
        {
            try
            {
                var viewModel = _newsService.GetNewsPageViewModel(page, 20);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                log.Error("Error in Index: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: News/5 or GET: News/Details/5
        public ActionResult Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    return RedirectToAction("Index", "News");
                }

                var viewModel = _newsService.GetFeedItemDetailsViewModel(id.Value, Server.MapPath);
                if (viewModel == null)
                {
                    return HttpNotFound();
                }

                return View(viewModel);
            }
            catch (Exception ex)
            {
                log.Error("Error in Details: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }
    }
}
