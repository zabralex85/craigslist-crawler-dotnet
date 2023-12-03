using Microsoft.Extensions.DependencyInjection;
using Zabr.Crawler.Scrapers.Enums;
using Zabr.Crawler.Scrapers.Implementations;
using Zabr.Crawler.Scrapers.Interfaces;

namespace Zabr.Crawler.Scrapers
{
    public interface IScraperFactory
    {
        IScraper CreateScraper(ResourceType resourceType);
    }

    public class ScraperFactory : IScraperFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public ScraperFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IScraper CreateScraper(ResourceType resourceType)
        {
            IScraper factoryResult = resourceType switch
            {
                ResourceType.GenericHttp => _serviceProvider.GetRequiredService<ICrawlerHttpService>(),
                ResourceType.GenericJs => _serviceProvider.GetRequiredService<ICrawlerJavaScriptService>(),
                ResourceType.Craigslist => _serviceProvider.GetRequiredService<ICraigslistScraper>(),
                ResourceType.LinkedIn => throw new NotImplementedException(),
                _ => throw new ArgumentException("Unrecognized resource type", nameof(resourceType))
            };

            return factoryResult;
        }
    }
}
