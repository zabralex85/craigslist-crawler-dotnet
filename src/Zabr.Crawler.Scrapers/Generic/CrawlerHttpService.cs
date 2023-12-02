using System.Text;
using Zabr.Crawler.Common.Models.Crawl;
using Zabr.Crawler.Scrapers.Interfaces;

namespace Zabr.Crawler.Scrapers.Generic
{
    public class CrawlerHttpService : ICrawlerHttpService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CrawlerHttpService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private static async Task<RootPage> ParseContentAsync(string url, Stream contentStream)
        {
            string content = string.Empty;
            using (var sr = new StreamReader(contentStream, Encoding.UTF8))
            {
                contentStream.Seek(0, SeekOrigin.Begin);
                content = await sr.ReadToEndAsync();
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

        public async Task<RootPage> GetPageAsync(string url, CancellationToken token)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers =
                {
                    { "Accept", "*/*" },
                    { "Cache-Control", "no-cache" },
                    { "User-Agent", "Mozilla/5.0 (Linux; Android 8.0.0; SM-G955U Build/R16NW) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/116.0.0.0 Mobile Safari/537.36" }
                }
            };

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, token);

            httpResponseMessage.EnsureSuccessStatusCode();

            await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync(token);
            var page = await ParseContentAsync(url, contentStream);

            return page;
        }
    }
}
