using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using log4net;
using Project_IT.Helpers;
using Project_IT.Models;
using Project_IT.Services.Interfaces;

namespace Project_IT.Services.Implementations
{
    public class SitemapService : ISitemapService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SitemapService));
        private static readonly object _fileLock = new object();
        private readonly ApplicationDbContext _db;

        public SitemapService(ApplicationDbContext db)
        {
            _db = db;
        }

        public SitemapService()
        {
            _db = null;
        }

        public void RegenerateSitemap()
        {
            lock (_fileLock)
            {
                try
                {
                    XNamespace ns = ConfigurationManager.AppSettings["Xmlns"] ?? "http://www.sitemaps.org/schemas/sitemap/0.9";
                    string baseUrl = (ConfigurationManager.AppSettings["BaseUrl"] ?? "").TrimEnd('/');
                    string now = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ss+00:00");
                    var locales = CultureHelper.Supported;

                    var root = new XElement(ns + "urlset");

                    // 1. Localized Home + Static Routes
                    var staticRoutes = GetStaticRoutes();
                    foreach (var locale in locales)
                    {
                        bool isDefaultLocale = locale.Equals(CultureHelper.Default, StringComparison.OrdinalIgnoreCase);
                        string localePrefix = "/" + locale.ToLowerInvariant();

                        root.Add(new XElement(ns + "url",
                            new XElement(ns + "loc", baseUrl + localePrefix),
                            new XElement(ns + "lastmod", now),
                            new XElement(ns + "priority", isDefaultLocale ? "1.0" : "0.8")
                        ));

                        foreach (var route in staticRoutes.Where(r => r != "/"))
                        {
                            root.Add(new XElement(ns + "url",
                                new XElement(ns + "loc", baseUrl + localePrefix + route),
                                new XElement(ns + "lastmod", now),
                                new XElement(ns + "priority", isDefaultLocale ? "0.8" : "0.64")
                            ));
                        }
                    }

                    // 2. Localized dynamic news items
                    using (var db = _db != null ? null : new ApplicationDbContext())
                    {
                        var context = _db ?? db;
                        var newsItems = context.FeedItems
                            .Where(x => x.IsPublished)
                            .Select(x => new { x.Id, x.CreatedOn })
                            .ToList();

                        foreach (var locale in locales)
                        {
                            bool isDefaultLocale = locale.Equals(CultureHelper.Default, StringComparison.OrdinalIgnoreCase);
                            string localePrefix = "/" + locale.ToLowerInvariant();

                            foreach (var item in newsItems)
                            {
                                root.Add(new XElement(ns + "url",
                                    new XElement(ns + "loc", $"{baseUrl}{localePrefix}/news/{item.Id}"),
                                    new XElement(ns + "lastmod", item.CreatedOn.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss+00:00")),
                                    new XElement(ns + "priority", isDefaultLocale ? "0.64" : "0.512")
                                ));
                            }
                        }
                    }

                    // 3. Write to physical file ~/sitemap.xml
                    string filePath = HttpContext.Current?.Server.MapPath("~/sitemap.xml")
                                      ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "sitemap.xml");

                    new XDocument(root).Save(filePath);
                }
                catch (Exception ex)
                {
                    log.Error("Error regenerating sitemap: " + ex.Message, ex);
                    throw;
                }
            }
        }

        private static List<string> GetStaticRoutes()
        {
            var controllerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IController).IsAssignableFrom(t) && !t.IsAbstract && t.Name.EndsWith("Controller"));

            var excludedControllers = new[] { "AccountController", "ManageController", "ErrorController", "AdminFileManagerController", "AdminReservationsController", "RootController" };
            var routes = new List<string>();

            foreach (var controller in controllerTypes.Where(c => !excludedControllers.Contains(c.Name)))
            {
                if (controller.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any())
                {
                    continue;
                }

                string controllerName = controller.Name.Replace("Controller", "");

                var actionMethods = controller.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .Where(m => typeof(ActionResult).IsAssignableFrom(m.ReturnType))
                    .Where(m => !m.GetCustomAttributes(typeof(HttpPostAttribute), false).Any())
                    .Where(m => !m.GetCustomAttributes(typeof(ChildActionOnlyAttribute), false).Any())
                    .Where(m => !m.GetCustomAttributes(typeof(AuthorizeAttribute), true).Any())
                    .Where(m => m.GetParameters().All(p => p.IsOptional));

                foreach (var action in actionMethods)
                {
                    string actionName = action.Name;

                    if (actionName.Equals("Error", StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    if (controllerName.Equals("Home", StringComparison.OrdinalIgnoreCase) && actionName.Equals("Index", StringComparison.OrdinalIgnoreCase))
                    {
                        routes.Add("/");
                    }
                    else if (actionName.Equals("Index", StringComparison.OrdinalIgnoreCase))
                    {
                        routes.Add($"/{controllerName.ToLower()}");
                    }
                    else
                    {
                        routes.Add($"/{controllerName.ToLower()}/{actionName.ToLower()}");
                    }
                }
            }

            return routes.Distinct().ToList();
        }
    }
}
