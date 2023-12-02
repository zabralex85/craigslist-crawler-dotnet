using System.Globalization;
using HtmlAgilityPack;
using Zabr.Craiglists.Crawler.Common.Models.Crawler;

namespace Zabr.Craiglists.Crawler.Consumer.Services
{
    public class CraiglistsDirectoryService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CraiglistsDirectoryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        private static DirectoryPage ParseHtmlContentAsync(string url, Stream contentStream)
        {
            var doc = new HtmlDocument();
            doc.Load(contentStream);

            var countNode = doc.DocumentNode.SelectSingleNode("//span[@class='cl-page-number']");
            var countNodeText = countNode.InnerText.Replace(",", "").Split("of")[3];

            var page = new DirectoryPage
            {
                Url = url,
                PageCount = int.Parse(countNodeText, NumberStyles.Integer),
                Pages = new List<Page>()
            };

            return page;
        }

        public async Task<DirectoryPage> GetPageAsync(string url, CancellationToken token)
        {
            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url)
            {
                Headers =
                {
                    { "Accept", "application/vnd.github.v3+json" },
                    { "User-Agent", "HttpRequestsConsoleSample" }
                }
            };

            var httpClient = _httpClientFactory.CreateClient();
            var httpResponseMessage = await httpClient.SendAsync(httpRequestMessage, token);

            httpResponseMessage.EnsureSuccessStatusCode();

            await using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync(token);
            return await Task.FromResult(ParseHtmlContentAsync(url, contentStream));
        }
    }
}
