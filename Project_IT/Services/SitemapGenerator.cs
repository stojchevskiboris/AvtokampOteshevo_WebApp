using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using log4net;
using Project_IT.Models;

namespace Project_IT.Services
{
    public static class SitemapGenerator
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SitemapGenerator));
        private static readonly object _fileLock = new object();

        public static void RegenerateSitemap()
        {
            lock (_fileLock)
            {
                try
                {
                    XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
                    string baseUrl = "https://www.otesevo.com";

                    var root = new XElement(ns + "urlset");

                    // 1. Static Routes (Scrapes Controllers/Views)
                    var staticRoutes = GetStaticRoutes();
                    foreach (var route in staticRoutes)
                    {
                        root.Add(new XElement(ns + "url",
                            new XElement(ns + "loc", baseUrl + route),
                            new XElement(ns + "changefreq", route == "/" ? "daily" : "weekly"),
                            new XElement(ns + "priority", route == "/" ? "1.0" : "0.8")
                        ));
                    }

                    // 2. Dynamic News Items
                    using (var db = new ApplicationDbContext())
                    {
                        var newsItems = db.FeedItems
                            .Where(x => x.IsPublished)
                            .Select(x => new { x.Id, x.CreatedOn })
                            .ToList();

                        foreach (var item in newsItems)
                        {
                            root.Add(new XElement(ns + "url",
                                new XElement(ns + "loc", $"{baseUrl}/News/{item.Id}"),
                                new XElement(ns + "lastmod", item.CreatedOn.ToString("yyyy-MM-dd")),
                                new XElement(ns + "changefreq", "weekly"),
                                new XElement(ns + "priority", "0.6")
                            ));
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
                }
            }
        }

        private static List<string> GetStaticRoutes()
        {
            var controllerTypes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => typeof(IController).IsAssignableFrom(t) && !t.IsAbstract && t.Name.EndsWith("Controller"));

            var excludedControllers = new[] { "AccountController", "ManageController", "ErrorController", "AdminFileManagerController", "AdminReservationsController" };
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
                        routes.Add($"/{controllerName}");
                    }
                    else
                    {
                        routes.Add($"/{controllerName}/{actionName}");
                    }
                }
            }

            return routes.Distinct().ToList();
        }
    }
}
