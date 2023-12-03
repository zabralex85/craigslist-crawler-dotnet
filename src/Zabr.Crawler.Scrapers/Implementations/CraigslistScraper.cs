using DotnetCraigslist;
using Zabr.Crawler.Scrapers.Enums;
using Zabr.Crawler.Scrapers.Interfaces;
using Zabr.Crawler.Scrapers.Models;

namespace Zabr.Crawler.Scrapers.Implementations
{
    public class CraigslistScraper : ICraigslistScraper, IScraper
    {
        public async Task<ScrapeResult[]> ScrapeAsync(ResourceType resourceType, string url, CancellationToken cancellationToken)
        {
            var client = new CraigslistClient();

            var request = new SearchCommunityRequest(
                "saltlakecity",
                SearchCommunityRequest.Categories.MissedConnections)
            {
                SearchDistance = 1050,
                Sort = SearchRequest.SortOrder.Oldest
            };

            var results = new List<ScrapeResult>();
            var searchResults = await client.SearchAsync(request, cancellationToken);

            foreach (var result in searchResults.Results)
            {
                results.Add(new ScrapeResult()
                {
                    Id = Guid.NewGuid(),
                    Url = result.PostingUrl.ToString(),
                    Content = result.Hood,
                });
            }

            return results.ToArray();
        }
    }
}
