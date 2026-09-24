using System.Web.Mvc;
using Unity;
using Unity.Lifetime;
using Unity.Mvc5;
using Project_IT.Models;
using Project_IT.Services.Implementations;
using Project_IT.Services.Interfaces;

namespace Project_IT
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
            var container = new UnityContainer();

            // Register ApplicationDbContext as HierarchicalLifetimeManager (PerRequest scoped)
            container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());

            // Register Services
            container.RegisterType<ISitemapService, SitemapService>(new HierarchicalLifetimeManager());
            container.RegisterType<IFeedItemService, FeedItemService>(new HierarchicalLifetimeManager());
            container.RegisterType<IReservationService, ReservationService>(new HierarchicalLifetimeManager());
            container.RegisterType<INewsService, NewsService>(new HierarchicalLifetimeManager());
            container.RegisterType<IFileManagerService, FileManagerService>(new HierarchicalLifetimeManager());

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}
