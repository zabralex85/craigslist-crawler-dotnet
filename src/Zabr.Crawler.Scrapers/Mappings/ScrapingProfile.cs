using AutoMapper;
using Zabr.Crawler.Scrapers.Models;

namespace Zabr.Crawler.Scrapers.Mappings
{
    public class ScrapingProfile : Profile
    {
        public ScrapingProfile()
        {
            CreateMap<SourceType, DestinationType>();
            // More mappings
        }
    }
}
