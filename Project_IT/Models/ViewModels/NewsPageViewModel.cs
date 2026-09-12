using System.Collections.Generic;

namespace Project_IT.Models
{
    public class NewsPageViewModel
    {
        public FeedItem FeaturedNews { get; set; }
        public List<FeedItem> GridNews { get; set; } = new List<FeedItem>();
        public List<FeedItem> Offers { get; set; } = new List<FeedItem>();

        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public int TotalItems { get; set; } = 0;

        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
