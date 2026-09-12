using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Project_IT.Models
{
        public class MediaItem
    {
        public string Url { get; set; }
        public bool IsVideo { get; set; }
    }
    public class FeedItemDetailsViewModel
    {
        public FeedItem FeedItem { get; set; }
        public List<MediaItem> GalleryMedia { get; set; } = new List<MediaItem>();
    }
}