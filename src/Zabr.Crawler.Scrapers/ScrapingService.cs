using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using Zabr.Crawler.Scrapers.Enums;
using Zabr.Crawler.Scrapers.Interfaces;
using Zabr.Crawler.Scrapers.Models;

namespace Zabr.Crawler.Scrapers
{
    public class ScrapingService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IMapper _mapper;

        public ScrapingService(IServiceProvider serviceProvider, IMapper mapper)
        {
            _serviceProvider = serviceProvider;
            _mapper = mapper;
        }

        public async Task<ScrapeResult> ScrapeResource(ResourceType resourceType)
        {
            var scraper = _serviceProvider.GetRequiredService<IScraper>();
            var result = await scraper.ScrapeAsync(resourceType);
            var mappedResult = _mapper.Map<ScrapeResult>(result);
            return mappedResult;
        }
    }
}
