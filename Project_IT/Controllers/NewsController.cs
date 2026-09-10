using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using log4net;
using Project_IT.Models;

namespace Project_IT.Controllers
{
    public class NewsController : Controller
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NewsController));
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        // GET: News
        public ActionResult Index(int page = 1)
        {
            try
            {
                if (page < 1)
                {
                    page = 1;
                }

                int pageSize = 20;

                // Query published feed items ordered by IsFeatured desc, CreatedOn desc
                var query = db.FeedItems
                    .Where(x => x.IsPublished)
                    .OrderByDescending(x => x.IsFeatured)
                    .ThenByDescending(x => x.CreatedOn);

                int totalItems = query.Count();
                int totalPages = (int)Math.Ceiling((double)totalItems / pageSize);
                if (totalPages < 1)
                {
                    totalPages = 1;
                }

                if (page > totalPages)
                {
                    page = totalPages;
                }

                var itemsForPage = query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                // Separate into Featured, Grid News/Events, and active Offers
                // 1. Featured News: First featured item on page, or first item if page 1 and is news/event
                FeedItem featured = itemsForPage.FirstOrDefault(x => x.IsFeatured);
                if (featured == null && page == 1)
                {
                    featured = itemsForPage.FirstOrDefault(x => x.Category == FeedCategory.News || x.Category == FeedCategory.Event);
                }

                // 2. Grid News/Events: Top 3 news or event items excluding featured
                var gridNews = itemsForPage
                    .Where(x => (x.Category == FeedCategory.News || x.Category == FeedCategory.Event) && (featured == null || x.Id != featured.Id))
                    .Take(3)
                    .ToList();

                // 3. Offers: Active offers (ValidTo is null or >= DateTime.UtcNow)
                DateTime now = DateTime.UtcNow;
                var offers = itemsForPage
                    .Where(x => x.Category == FeedCategory.Offer && (!x.ValidTo.HasValue || x.ValidTo.Value >= now))
                    .ToList();

                var viewModel = new NewsPageViewModel
                {
                    FeaturedNews = featured,
                    GridNews = gridNews,
                    Offers = offers,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalItems = totalItems
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                log.Error("Error in Index: " + ex.Message, ex);
                return RedirectToAction("Error", "Home");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
