using System.Text;
using Zabr.Crawler.Common.Models.Crawl;
using Zabr.Crawler.Scrapers.Enums;
using Zabr.Crawler.Scrapers.Models;

namespace Zabr.Crawler.Scrapers.Implementations.GenericHttp
{
    public class CrawlerHttpScraper : ICrawlerHttpScraper
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CrawlerHttpScraper(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private static async Task<RootPage> ParseContentAsync(string url, Stream contentStream)
        {
            string content = string.Empty;

            using (var reader = new StreamReader(contentStream, Encoding.UTF8))
            {
                var buffer = new char[2048];
                int read;

                while ((read = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    content += new string(buffer, 0, read);
                }
            }

            var page = new RootPage
            {
                Url = url,
                PageCount = 0,
                Pages = new List<InternalPage>(),
                Content = content
            };

            return page;
        }

        public async Task<ScrapeResult[]> ScrapeAsync(ResourceType resourceType, string url, CancellationToken token)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers =
                {
                    { "Accept", "*/*" },
                    { "User-Agent", "Mozilla/5.0 (Linux; Android 8.0.0; SM-G955U Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Mobile Safari/537.36" }
                }
            };

            var httpClient = _httpClientFactory.CreateClient("NoAutomaticCookies");
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, token);

            httpResponseMessage.EnsureSuccessStatusCode();

            var contentLengthHeader = httpResponseMessage.Content.Headers.ContentLength;
            var isChunked = httpResponseMessage.Headers.TransferEncodingChunked ?? false;

            var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync(token);
            var page = await ParseContentAsync(url, contentStream);

            return new ScrapeResult[]
            {
                new ScrapeResult
                {
                    Content = page.Content,
                    Url = url,
                    Id = Guid.NewGuid()
                }
            };
        }
    }
}
