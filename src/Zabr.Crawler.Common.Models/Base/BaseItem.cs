using System.Text.Json.Serialization;

namespace Zabr.Crawler.Common.Models.Base;

public class BaseItem
{
    [JsonPropertyName("Url")]
    public string Url { get; init; } = string.Empty;
}
