using System.Text.Json.Serialization;
using Zabr.Crawler.Common.Models.Base;

namespace Zabr.Crawler.Common.Models.Crawl;

public class RootPage : BasePage
{
    [JsonPropertyName("PageCount")]
    public int PageCount { get; init; } = 0;

    [JsonPropertyName("Pages")]
    public List<InternalPage> Pages { get; init; } = new List<InternalPage>();
}
