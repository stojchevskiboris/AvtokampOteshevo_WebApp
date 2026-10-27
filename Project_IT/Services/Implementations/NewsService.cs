using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using Project_IT.Helpers;
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

                var currentCulture = CultureHelper.Current;

                return new NewsPageViewModel
                {
                    FeaturedNews = LocalizeFeedItem(featured, currentCulture),
                    GridNews = gridNews.Select(x => LocalizeFeedItem(x, currentCulture)).ToList(),
                    Offers = offers.Select(x => LocalizeFeedItem(x, currentCulture)).ToList(),
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
                    FeedItem = LocalizeFeedItem(feedItem, CultureHelper.Current),
                    GalleryMedia = mediaList
                };
            }
            catch (Exception ex)
            {
                log.Error($"Error getting FeedItemDetailsViewModel for ID {id}: " + ex.Message, ex);
                throw;
            }
        }

        private static FeedItem LocalizeFeedItem(FeedItem feedItem, string currentCulture)
        {
            if (feedItem == null)
            {
                return null;
            }

            var localized = new FeedItem
            {
                Id = feedItem.Id,
                Title = feedItem.Title,
                TitleEn = feedItem.TitleEn,
                Subtitle = feedItem.Subtitle,
                SubtitleEn = feedItem.SubtitleEn,
                Description = feedItem.Description,
                DescriptionEn = feedItem.DescriptionEn,
                ImageUrl = feedItem.ImageUrl,
                Category = feedItem.Category,
                LabelBadge = feedItem.LabelBadge,
                LabelBadgeEn = feedItem.LabelBadgeEn,
                OfferText = feedItem.OfferText,
                OfferTextEn = feedItem.OfferTextEn,
                ButtonText = feedItem.ButtonText,
                ButtonTextEn = feedItem.ButtonTextEn,
                ActionUrl = feedItem.ActionUrl,
                ValidTo = feedItem.ValidTo,
                IsFeatured = feedItem.IsFeatured,
                IsPublished = feedItem.IsPublished,
                CreatedOn = feedItem.CreatedOn,
                GalleryPath = feedItem.GalleryPath
            };

            var normalizedCulture = NormalizeCulture(currentCulture);
            var isMacedonian = string.Equals(normalizedCulture, CultureHelper.Default, StringComparison.OrdinalIgnoreCase);

            if (!isMacedonian)
            {
                localized.Title = string.IsNullOrWhiteSpace(localized.TitleEn) ? localized.Title : localized.TitleEn;
                localized.Subtitle = string.IsNullOrWhiteSpace(localized.SubtitleEn) ? localized.Subtitle : localized.SubtitleEn;
                localized.Description = string.IsNullOrWhiteSpace(localized.DescriptionEn) ? localized.Description : localized.DescriptionEn;
                localized.LabelBadge = string.IsNullOrWhiteSpace(localized.LabelBadgeEn) ? localized.LabelBadge : localized.LabelBadgeEn;
                localized.OfferText = string.IsNullOrWhiteSpace(localized.OfferTextEn) ? localized.OfferText : localized.OfferTextEn;
                localized.ButtonText = string.IsNullOrWhiteSpace(localized.ButtonTextEn) ? localized.ButtonText : localized.ButtonTextEn;
            }

            localized.ActionUrl = LocalizeActionUrl(localized.ActionUrl, normalizedCulture);

            return localized;
        }

        private static string NormalizeCulture(string currentCulture)
        {
            var culture = string.IsNullOrWhiteSpace(currentCulture)
                ? CultureHelper.Default
                : currentCulture.Trim().ToLowerInvariant();

            if (culture.Contains("-"))
            {
                culture = culture.Split('-')[0];
            }

            return CultureHelper.IsSupported(culture) ? culture : CultureHelper.Default;
        }

        private static string LocalizeActionUrl(string actionUrl, string culture)
        {
            if (string.IsNullOrWhiteSpace(actionUrl))
            {
                return actionUrl;
            }

            var normalized = actionUrl.Trim();
            if (normalized.StartsWith("~/"))
            {
                normalized = normalized.Substring(1);
            }

            if (normalized.StartsWith("#", StringComparison.Ordinal) ||
                normalized.StartsWith("//", StringComparison.Ordinal) ||
                normalized.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase) ||
                normalized.StartsWith("tel:", StringComparison.OrdinalIgnoreCase) ||
                Uri.TryCreate(normalized, UriKind.Absolute, out _))
            {
                return normalized;
            }

            if (!normalized.StartsWith("/"))
            {
                normalized = "/" + normalized;
            }

            foreach (var supportedCulture in CultureHelper.Supported)
            {
                var existingPrefix = "/" + supportedCulture;
                if (normalized.Equals(existingPrefix, StringComparison.OrdinalIgnoreCase))
                {
                    return "/" + culture;
                }

                if (normalized.StartsWith(existingPrefix + "/", StringComparison.OrdinalIgnoreCase))
                {
                    return "/" + culture + normalized.Substring(existingPrefix.Length);
                }
            }

            return "/" + culture + normalized;
        }
    }
}
