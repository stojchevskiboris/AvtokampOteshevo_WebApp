using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project_IT.Controllers
{
    [AllowAnonymous]
    public class HomeController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(HomeController));

        public ActionResult Index()
        {
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

        public ActionResult About()
        {
            try
            {
                ViewBag.Message = "Your application description page.";

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
                Response.StatusCode = 500;
            }
            catch
            {
            }
            return View();
        }
    }
}
