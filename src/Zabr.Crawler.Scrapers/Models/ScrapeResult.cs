namespace Zabr.Crawler.Scrapers.Models
{
    public class ScrapeResult
    {
        public Guid Id { get; set; }
        public string Url { get; set; }
        public string Content { get; set; }

        public ScrapeResult() { }
    }
}
