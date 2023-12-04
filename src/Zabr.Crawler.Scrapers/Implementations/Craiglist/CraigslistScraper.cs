using System.Text.Json;
using System.Text.Json.Serialization;
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

            // Split into batches and process each batch
            var batches = searchResults.Results
                .Select((result, index) => new { result, index })
                .GroupBy(x => x.index / 5)
                .Select(group => group.Select(x => x.result).ToList());

            foreach (var batch in batches)
            {
                var tasks = batch.Select(result => client.GetPostingAsync(
                    new PostingRequest(result.PostingUrl.ToString()), cancellationToken));

                var postings = await Task.WhenAll(tasks);

                results.AddRange(postings.Select(ScrapeResult));

                // Optional delay to avoid rate limits
                // await Task.Delay(1000);
            }

            return results.ToArray();
        }

        private static ScrapeResult ScrapeResult(Posting posting)
        {
            var result = new ScrapeResult
            {
                Id = Guid.NewGuid(),
                Url = posting.PostingUrl.ToString()
            };

            var json = new
            {
                Id = posting.Id,
                Price = posting.Price,
                PostedOn = posting.PostedOn,
                Location = posting.Location,
                Title = posting.Title,
                Description = posting.Description
            };

            result.Content = JsonSerializer.Serialize(json);

            return result;
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
