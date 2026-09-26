using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Project_IT.Models;
using Project_IT.Services.Interfaces;

namespace Project_IT.Services.Implementations
{
    public class NewsService : INewsService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(NewsService));
        private readonly ApplicationDbContext _db;

        public NewsService(ApplicationDbContext db)
        {
            _db = db;
        }

        public NewsPageViewModel GetNewsPageViewModel(int page, int pageSize = 20)
        {
            try
            {
                if (page < 1)
                {
                    page = 1;
                }

                var query = _db.FeedItems
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

                FeedItem featured = itemsForPage.FirstOrDefault(x => x.IsFeatured);
                if (featured == null && page == 1)
                {
                    featured = itemsForPage.FirstOrDefault(x => x.Category == FeedCategory.News || x.Category == FeedCategory.Event);
                }

                var gridNews = itemsForPage
                    .Where(x => (x.Category == FeedCategory.News || x.Category == FeedCategory.Event) && (featured == null || x.Id != featured.Id))
                    .Take(3)
                    .ToList();

                DateTime now = DateTime.UtcNow;
                var offers = itemsForPage
                    .Where(x => x.Category == FeedCategory.Offer && (!x.ValidTo.HasValue || x.ValidTo.Value >= now))
                    .ToList();

                return new NewsPageViewModel
                {
                    FeaturedNews = featured,
                    GridNews = gridNews,
                    Offers = offers,
                    CurrentPage = page,
                    TotalPages = totalPages,
                    PageSize = pageSize,
                    TotalItems = totalItems
                };
            }
            catch (Exception ex)
            {
                log.Error("Error generating NewsPageViewModel: " + ex.Message, ex);
                throw;
            }
        }

        public FeedItemDetailsViewModel GetFeedItemDetailsViewModel(int id, Func<string, string> mapPath)
        {
            try
            {
                FeedItem feedItem = _db.FeedItems.Find(id);
                if (feedItem == null || !feedItem.IsPublished)
                {
                    return null;
                }

                string relativePath = !string.IsNullOrWhiteSpace(feedItem.GalleryPath)
                    ? feedItem.GalleryPath
                    : $"/Content/Gallery/{feedItem.Id}";

                string absolutePath = mapPath != null ? mapPath(relativePath) : null;
                var mediaList = new List<MediaItem>();

                if (!string.IsNullOrEmpty(absolutePath) && Directory.Exists(absolutePath))
                {
                    var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".mp4", ".webm" };
                    var videoExtensions = new[] { ".mp4", ".webm" };

                    var files = Directory.GetFiles(absolutePath)
                        .Where(f => validExtensions.Contains(Path.GetExtension(f).ToLower()))
                        .OrderBy(f => f);

                    foreach (var file in files)
                    {
                        string fileName = Path.GetFileName(file);
                        string ext = Path.GetExtension(file).ToLower();

                        mediaList.Add(new MediaItem
                        {
                            Url = $"{relativePath.TrimEnd('/')}/{fileName}",
                            IsVideo = videoExtensions.Contains(ext)
                        });
                    }
                }

                return new FeedItemDetailsViewModel
                {
                    FeedItem = feedItem,
                    GalleryMedia = mediaList
                };
            }
            catch (Exception ex)
            {
                log.Error($"Error getting FeedItemDetailsViewModel for ID {id}: " + ex.Message, ex);
                throw;
            }
        }
    }
}
