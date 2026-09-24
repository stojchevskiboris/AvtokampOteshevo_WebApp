using System;
using Project_IT.Models;

namespace Project_IT.Services.Interfaces
{
    public interface INewsService
    {
        NewsPageViewModel GetNewsPageViewModel(int page, int pageSize = 20);
        FeedItemDetailsViewModel GetFeedItemDetailsViewModel(int id, Func<string, string> mapPath);
    }
}
