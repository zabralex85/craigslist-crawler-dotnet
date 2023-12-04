using System.Text.RegularExpressions;
using AutoMapper;
using Zabr.Crawler.Scrapers.Enums;
using Zabr.Crawler.Scrapers.Interfaces;
using Zabr.Crawler.Scrapers.Models;

namespace Zabr.Crawler.Scrapers
{
    public class ScrapingService : IScrapingService
    {
        private readonly IScraperFactory _scraperFactory;
        private readonly IMapper _mapper;

        public ScrapingService(IMapper mapper, IScraperFactory scraperFactory)
        {
            _mapper = mapper;
            _scraperFactory = scraperFactory;
        }

        public async Task<ScrapeResult[]> ScrapeResourceAsync(ResourceType resourceType, string url, CancellationToken token)
        {
            var scraper = _scraperFactory.CreateScraper(resourceType);
            var result = await scraper.ScrapeAsync(resourceType, url, token);
            var mappedResult = _mapper.Map<ScrapeResult[]>(result);
            return mappedResult;
        }

        public ResourceType RecognizeResource(string contentUrl)
        {
            if (Regex.IsMatch(contentUrl, @"craigslist"))
            {
                return ResourceType.Craigslist;
            }

            return ResourceType.GenericHttp;
        }
    }
}
