using Zabr.Crawler.Scrapers.Enums;
using Zabr.Crawler.Scrapers.Models;

namespace Zabr.Crawler.Scrapers.Interfaces
{
    public interface IScrapingService
    {
        Task<ScrapeResult[]> ScrapeResourceAsync(ResourceType resourceType, string url, CancellationToken token);
        ResourceType RecognizeResource(string contentUrl);
    }
}
