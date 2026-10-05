using System.Web;
using System.Web.Mvc;
using Project_IT.Helpers;

namespace Project_IT.Controllers
{
    public class RootController : Controller
    {
        public ActionResult Index()
        {
            var lang = CultureHelper.Detect(Request);
            CultureHelper.Apply(lang);
            Response.Cache.SetCacheability(HttpCacheability.Private);
            Response.AppendHeader("Vary", "Cookie");
            return Redirect("/" + lang + "/");
        }
    }
}
