using System.Text.Json.Serialization;

namespace Zabr.Craiglists.Crawler.Common.Models.Base;

public class BaseItem
{
    [JsonPropertyName("Url")]
    public string Url { get; init; }
}
