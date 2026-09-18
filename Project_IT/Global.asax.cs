using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using Project_IT.Controllers;
using Project_IT.Services;

namespace Project_IT
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static log4net.ILog Log { get; set; }
        ILog log = log4net.LogManager.GetLogger(typeof(ReservationsController));

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            log4net.Config.XmlConfigurator.Configure();

            // Generate initial sitemap on app startup
            SitemapGenerator.RegenerateSitemap();

            // Register 24-hour background task
            RegisterDailySitemapTask();
        }

        private static void RegisterDailySitemapTask()
        {
            HttpRuntime.Cache.Insert(
                "DailySitemapTask",
                "dummy_value",
                null,
                DateTime.Now.AddDays(1), // Runs 24 hours from now
                System.Web.Caching.Cache.NoSlidingExpiration,
                System.Web.Caching.CacheItemPriority.NotRemovable,
                new System.Web.Caching.CacheItemRemovedCallback(SitemapTaskCallback)
            );
        }

        private static void SitemapTaskCallback(string key, object value, System.Web.Caching.CacheItemRemovedReason reason)
        {
            // Re-generate full sitemap (static views + dynamic news)
            SitemapGenerator.RegenerateSitemap();

            // Re-register the daily task loop
            RegisterDailySitemapTask();
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            bool isRedirectEnabled = bool.TryParse(ConfigurationManager.AppSettings["EnableDomainRedirect"], out var enabled) && enabled;

            if (isRedirectEnabled)
            {
                string currentHost = Request.Url.Host;

                if (currentHost.Equals("otesevo.bsite.net", StringComparison.OrdinalIgnoreCase))
                {
                    string newUrl = "https://otesevo.com" + Request.Url.PathAndQuery;

                    Response.RedirectPermanent(newUrl, endResponse: true);
                }
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex == null)
            {
                Response.Redirect($"~/Home/Error");
            }
            log.Error(ex.Message);
            Server.ClearError();
            Response.Redirect($"~/Home/Error");

        }

    }
}