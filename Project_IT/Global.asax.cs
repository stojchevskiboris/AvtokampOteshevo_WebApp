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
        protected void Application_Error(object sender, EventArgs e)
        {        
            var ex = Server.GetLastError();
            if (ex == null) return;
            log.Error(ex.Message);
        }
    }
}
