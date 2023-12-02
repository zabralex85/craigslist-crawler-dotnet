using System.Text.Json.Serialization;
using Zabr.Crawler.Common.Models.Base;

namespace Zabr.Crawler.Common.Models.Crawl;

public abstract class InternalPage : BasePage
{
    [JsonPropertyName("LatLng")]
    public LatLng LatLng { get; init; } = new LatLng(0, 0);
}
