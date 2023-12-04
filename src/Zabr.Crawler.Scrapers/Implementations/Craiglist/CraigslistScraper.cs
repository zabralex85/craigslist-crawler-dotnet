using System.Text.RegularExpressions;
using DotnetCraigslist;
using Zabr.Crawler.Scrapers.Enums;
using Zabr.Crawler.Scrapers.Models;

namespace Zabr.Crawler.Scrapers.Implementations.Craiglist
{
    public class CraigslistScraper : ICraigslistScraper
    {
        public async Task<ScrapeResult[]> ScrapeAsync(ResourceType resourceType, string url, CancellationToken cancellationToken)
        {
            string city = ExtractCityFromUrl(url);

            var client = new CraigslistClient();

            var request = new SearchCommunityRequest(city, SearchCommunityRequest.Categories.MissedConnections)
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

        private string ExtractCityFromUrl(string url)
        {
            return Regex.Match(url, @"https:\/\/(.*?)\.craigslist\.org").Value;
        }
    }
}
