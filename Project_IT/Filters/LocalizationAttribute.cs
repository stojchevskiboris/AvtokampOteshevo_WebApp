using System;
using System.Web;
using System.Web.Mvc;
using Project_IT.Helpers;

namespace Project_IT.Filters
{
    public class LocalizationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext ctx)
        {
            if (ctx.ActionDescriptor.ControllerDescriptor.ControllerName.Equals("Root", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var req = ctx.HttpContext.Request;
            var culture = ctx.RouteData.Values["culture"] as string;

            if (culture == null)   // legacy URL: permanent redirect to default Macedonian version
            {
                ctx.Result = new RedirectResult("/" + CultureHelper.Default + req.RawUrl, true);
                return;
            }

            CultureHelper.Apply(culture);

            if (req.Cookies["lang"]?.Value != culture)
            {
                ctx.HttpContext.Response.Cookies.Add(new HttpCookie("lang", culture)
                {
                    Expires = DateTime.UtcNow.AddYears(1),
                    HttpOnly = true,
                    Secure = req.IsSecureConnection,
                    SameSite = SameSiteMode.Lax
                });
            }
        }
    }
}
