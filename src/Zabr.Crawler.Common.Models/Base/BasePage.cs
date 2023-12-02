using System.Text.Json.Serialization;

namespace Zabr.Crawler.Common.Models.Base;

public class BasePage : BaseItem
{
    [JsonPropertyName("Content")]
    public string Content { get; set; } = string.Empty;
}
