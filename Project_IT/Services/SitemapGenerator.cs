using System.Web.Mvc;
using Project_IT.Services.Implementations;
using Project_IT.Services.Interfaces;

namespace Project_IT.Services
{
    public static class SitemapGenerator
    {
        public static void RegenerateSitemap()
        {
            var sitemapService = DependencyResolver.Current?.GetService<ISitemapService>()
                                 ?? new SitemapService();
            sitemapService.RegenerateSitemap();
        }
    }
}
