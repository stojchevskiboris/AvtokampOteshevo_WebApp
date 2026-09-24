using Project_IT.Models;
using Project_IT.Services.Implementations;
using Project_IT.Services.Interfaces;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Lifetime;

namespace Project_IT
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // 1. Register DbContext (Per HTTP Request)
            container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());

            // 2. Register Services
            container.RegisterType<ISitemapService, SitemapService>(new HierarchicalLifetimeManager());
            container.RegisterType<IFeedItemService, FeedItemService>(new HierarchicalLifetimeManager());
            container.RegisterType<IReservationService, ReservationService>(new HierarchicalLifetimeManager());
            container.RegisterType<INewsService, NewsService>(new HierarchicalLifetimeManager());
            container.RegisterType<IFileManagerService, FileManagerService>(new HierarchicalLifetimeManager());

            // 3. Set Unity as the ASP.NET MVC Dependency Resolver
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}