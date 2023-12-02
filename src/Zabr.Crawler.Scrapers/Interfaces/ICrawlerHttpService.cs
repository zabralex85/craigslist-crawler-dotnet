using Zabr.Crawler.Common.Models.Crawl;

namespace Zabr.Crawler.Scrapers.Interfaces
{
    public interface ICrawlerHttpService
    {
        Task<RootPage> GetPageAsync(string url, CancellationToken token);
    }
}
