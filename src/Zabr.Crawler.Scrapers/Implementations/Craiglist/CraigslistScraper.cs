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
                var posting = await client.GetPostingAsync(new PostingRequest(result.PostingUrl.ToString()), cancellationToken);
                results.Add(new ScrapeResult
                {
                    Id = Guid.NewGuid(),
                    Url = result.PostingUrl.ToString(),
                    Content = $"{posting.FullTitle} || {posting.Description}"
                });
            }

            return results.ToArray();
        }

        private string ExtractCityFromUrl(string url)
        {
            var result = Regex.Match(url, @"https:\/\/(.*?)\.craigslist\.org").Value;
            result = result.Replace("https://", "");
            result = result.Replace(".craigslist.org", "");
            return result;
        }
    }
}
