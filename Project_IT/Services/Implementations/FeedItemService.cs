using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using log4net;
using Project_IT.Models;
using Project_IT.Services.Interfaces;

namespace Project_IT.Services.Implementations
{
    public class FeedItemService : IFeedItemService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(FeedItemService));
        private readonly ApplicationDbContext _db;
        private readonly ISitemapService _sitemapService;

        public FeedItemService(ApplicationDbContext db, ISitemapService sitemapService)
        {
            _db = db;
            _sitemapService = sitemapService;
        }

        public IEnumerable<FeedItem> GetAllFeedItems()
        {
            try
            {
                return _db.FeedItems.OrderByDescending(f => f.CreatedOn).ToList();
            }
            catch (Exception ex)
            {
                log.Error("Error fetching all feed items: " + ex.Message, ex);
                throw;
            }
        }

        public FeedItem GetFeedItemById(int id)
        {
            try
            {
                return _db.FeedItems.Find(id);
            }
            catch (Exception ex)
            {
                log.Error($"Error fetching feed item with ID {id}: " + ex.Message, ex);
                throw;
            }
        }

        public void CreateFeedItem(FeedItem feedItem)
        {
            try
            {
                _db.FeedItems.Add(feedItem);
                _db.SaveChanges();

                _sitemapService?.RegenerateSitemap();
            }
            catch (Exception ex)
            {
                log.Error("Error creating feed item: " + ex.Message, ex);
                throw;
            }
        }

        public void UpdateFeedItem(FeedItem feedItem)
        {
            try
            {
                _db.Entry(feedItem).State = EntityState.Modified;
                _db.SaveChanges();

                _sitemapService?.RegenerateSitemap();
            }
            catch (Exception ex)
            {
                log.Error($"Error updating feed item with ID {feedItem?.Id}: " + ex.Message, ex);
                throw;
            }
        }

        public void DeleteFeedItem(int id)
        {
            try
            {
                var feedItem = _db.FeedItems.Find(id);
                if (feedItem != null)
                {
                    _db.FeedItems.Remove(feedItem);
                    _db.SaveChanges();

                    _sitemapService?.RegenerateSitemap();
                }
            }
            catch (Exception ex)
            {
                log.Error($"Error deleting feed item with ID {id}: " + ex.Message, ex);
                throw;
            }
        }
    }
}
