using Zabr.Craiglists.Crawler.Data.Common;

namespace Zabr.Craiglists.Crawler.Data.Entities
{
    public class Page : BaseEntity<int>
    {
        public string Url { get; private set; } = string.Empty;
        public DateTime Date { get; private set; }
        public string Response { get; private set; } = string.Empty;

        public Page(int id, string url, DateTime date, string response) : base(id)
        {
            Id = id;
            Url = url;
            Date = date;
            Response = response;
        }
    }
}
