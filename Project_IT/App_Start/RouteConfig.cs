using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Project_IT
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.LowercaseUrls = true;

            string cultureConstraint = "mk|en|sq|sr|de|pl|it|el|es";

            // Localized custom routes
            routes.MapRoute(
                name: "LocalizedAdminReservations",
                url: "{culture}/Admin/Reservations/{action}/{id}",
                defaults: new { controller = "AdminReservations", action = "Index", id = UrlParameter.Optional },
                constraints: new { culture = cultureConstraint }
            );

            routes.MapRoute(
                name: "LocalizedNewsDetails",
                url: "{culture}/News/{id}",
                defaults: new { controller = "News", action = "Details" },
                constraints: new { culture = cultureConstraint, id = @"\d+" }
            );

            routes.MapRoute(
                name: "Localized",
                url: "{culture}/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                constraints: new { culture = cultureConstraint }
            );

            // Root URL "/" points to RootController for Accept-Language / cookie detection
            routes.MapRoute(
                name: "Root",
                url: "",
                defaults: new { controller = "Root", action = "Index" }
            );

            // Legacy non-localized URLs (e.g. /Home/About) redirected via LocalizationAttribute 301
            routes.MapRoute(
                name: "LegacyAdminReservations",
                url: "Admin/Reservations/{action}/{id}",
                defaults: new { controller = "AdminReservations", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "LegacyNewsDetails",
                url: "News/{id}",
                defaults: new { controller = "News", action = "Details" },
                constraints: new { id = @"\d+" }
            );

            routes.MapRoute(
                name: "Legacy",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
            );

            routes.MapRoute(
                name: "NotFoundFallback",
                url: "{*url}",
                defaults: new { controller = "Home", action = "Index" }
            );
        }
    }
}
