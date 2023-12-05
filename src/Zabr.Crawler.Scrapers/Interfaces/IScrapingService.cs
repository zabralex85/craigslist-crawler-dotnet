using Zabr.Crawler.Scrapers.Enums;
using Zabr.Crawler.Scrapers.Models;

namespace Zabr.Crawler.Scrapers.Interfaces
{
    public interface IScrapingService
    {
        Task<ScrapeResult[]> ScrapeResourceAsync(
            ResourceType resourceType,
            string url,
            HashSet<string>? processedPages = null,
            CancellationToken token = default);
        ResourceType RecognizeResource(string contentUrl);
    }
}
