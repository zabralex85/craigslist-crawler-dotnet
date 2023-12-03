using System.Text;
using Microsoft.Playwright;
using Zabr.Crawler.Common.Models.Crawl;
using Zabr.Crawler.Scrapers.Enums;
using Zabr.Crawler.Scrapers.Interfaces;
using Zabr.Crawler.Scrapers.Models;

namespace Zabr.Crawler.Scrapers.Implementations
{
    public class CrawlerJavaScriptService : ICrawlerJavaScriptService, IAsyncDisposable, IScraper
    {
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;

        public CrawlerJavaScriptService()
        {
            SetUpObjectsAsync().GetAwaiter().GetResult();
        }

        private async Task SetUpObjectsAsync()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync();
            _context = await _browser.NewContextAsync();
            _page = await _context.NewPageAsync();
        }

        private async Task<RootPage> ParseContentAsync(string url, IResponse? response)
        {
            var page = new RootPage
            {
                Url = url,
                PageCount = 0,
                Pages = new List<InternalPage>(),
                Content = String.Empty
            };

            if (response != null)
            {
                byte[] body = await response.BodyAsync().ConfigureAwait(false);
                page.Content = Encoding.UTF8.GetString(body);
            }

            return page;
        }

        public async Task<ScrapeResult[]> ScrapeAsync(ResourceType resourceType, string url, CancellationToken cancellationToken)
        {
            var response = await _page.GotoAsync(url).ConfigureAwait(false);
            var result = await ParseContentAsync(url, response).ConfigureAwait(false);

            var page = new ScrapeResult
            {
                Id = Guid.NewGuid(),
                Url = result.Url,
                Content = result.Content
            };

            return new ScrapeResult[] { page };
        }

        public async ValueTask DisposeAsync()
        {
            // Perform async cleanup.
            await _page.CloseAsync().ConfigureAwait(false);
            await _context.CloseAsync().ConfigureAwait(false);
            await _browser.CloseAsync().ConfigureAwait(false);
            
            // Dispose of unmanaged resources.
            Dispose(false);

            // Suppress finalization.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                _playwright.Dispose();
                _playwright = null;
            }
        }
    }
}
