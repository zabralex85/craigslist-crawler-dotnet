using Zabr.Crawler.Scrapers.Enums;
using Zabr.Crawler.Scrapers.Models;

namespace Zabr.Crawler.Scrapers.Interfaces
{
    public interface IScraper
    {
        public Task<ScrapeResult[]> ScrapeAsync(
            ResourceType resourceType,
            string url,
            HashSet<string>? processedPages = null,
            CancellationToken cancellationToken = default);
    }
}
