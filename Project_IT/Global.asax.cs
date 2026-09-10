using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using Project_IT.Controllers;

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