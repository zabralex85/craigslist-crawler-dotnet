using System.Text.Json.Serialization;

namespace Zabr.Craiglists.Crawler.Common.Models.Base;

public class BasePage : BaseItem
{
    [JsonPropertyName("Content")]
    public string Content { get; init; }
}
