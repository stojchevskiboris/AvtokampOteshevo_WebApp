using log4net;
using Project_IT.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

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

        // Redirects requests from the old bsite.net domain to the new otesevo.com domain
        // Comment out when publishing to stage
        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string currentHost = Request.Url.Host;
            // Intercepts any request hitting the old bsite.net domain
            if (currentHost.Equals("otesevo.bsite.net", StringComparison.OrdinalIgnoreCase))
            {
                string newUrl = "https://otesevo.com" + Request.Url.PathAndQuery;
                                Response.Clear();
                Response.StatusCode = 301;
                Response.Status = "301 Moved Permanently";
                Response.RedirectLocation = newUrl;
                Response.End();
            }
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            if (ex == null) return;
            log.Error(ex.Message);
        }
    }
}
