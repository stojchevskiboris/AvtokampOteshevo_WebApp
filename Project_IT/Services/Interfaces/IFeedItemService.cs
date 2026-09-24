using System.Collections.Generic;
using Project_IT.Models;

namespace Project_IT.Services.Interfaces
{
    public interface IFeedItemService
    {
        IEnumerable<FeedItem> GetAllFeedItems();
        FeedItem GetFeedItemById(int id);
        void CreateFeedItem(FeedItem feedItem);
        void UpdateFeedItem(FeedItem feedItem);
        void DeleteFeedItem(int id);
    }
}
