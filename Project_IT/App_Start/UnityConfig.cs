using System.Web.Mvc;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Mvc5;
using Project_IT.Controllers;
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

            // 1. Force Unity to use the parameterless constructor for AccountController
            container.RegisterType<AccountController>(new InjectionConstructor());

            // 2. Register DbContext (Per HTTP Request)
            container.RegisterType<ApplicationDbContext>(new HierarchicalLifetimeManager());

            // 3. Register Application Services
            container.RegisterType<ISitemapService, SitemapService>(new HierarchicalLifetimeManager());
            container.RegisterType<IFeedItemService, FeedItemService>(new HierarchicalLifetimeManager());
            container.RegisterType<IReservationService, ReservationService>(new HierarchicalLifetimeManager());
            container.RegisterType<INewsService, NewsService>(new HierarchicalLifetimeManager());
            container.RegisterType<IFileManagerService, FileManagerService>(new HierarchicalLifetimeManager());

            // 4. Set Unity as the ASP.NET MVC Dependency Resolver
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}