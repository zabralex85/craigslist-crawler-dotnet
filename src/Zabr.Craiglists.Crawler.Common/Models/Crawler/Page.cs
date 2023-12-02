using System.Text.Json.Serialization;
using Zabr.Craiglists.Crawler.Common.Models.Base;

namespace Zabr.Craiglists.Crawler.Common.Models.Crawler;

public abstract class Page : BasePage
{
    [JsonPropertyName("LatLng")]
    public LatLng LatLng { get; init; } = new LatLng(0, 0);
}
