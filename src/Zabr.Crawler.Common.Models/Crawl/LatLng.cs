using System.Text.Json.Serialization;

namespace Zabr.Crawler.Common.Models.Crawl;

public class LatLng
{
    [JsonPropertyName("Latitude")]
    public string Latitude { get; init; }

    [JsonPropertyName("Longitude")]
    public string Longitude { get; init; }

    public LatLng(float lat, float lon)
    {
        Latitude = lat.ToString("F");
        Longitude = lon.ToString("F");
    }
}
