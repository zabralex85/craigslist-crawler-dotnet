using System.Text.Json.Serialization;
using Zabr.Craiglists.Crawler.Common.Models.Base;

namespace Zabr.Craiglists.Crawler.Common.Models.Crawler;

public class DirectoryPage : BasePage
{
    [JsonPropertyName("PageCount")]
    public int PageCount { get; init; } = 0;

    [JsonPropertyName("Pages")]
    public List<Page> Pages { get; init; } = new List<Page>();
}
