using System.Text.Json.Serialization;

namespace Reddit.Client.Dtos;

public record ListingData<T>(
    [property: JsonPropertyName("after")] string After,
    [property: JsonPropertyName("dist")] int? Dist,
    [property: JsonPropertyName("modhash")] object Modhash,
    [property: JsonPropertyName("geo_filter")] string GeoFilter,
    [property: JsonPropertyName("children")] IReadOnlyList<T> Children,
    [property: JsonPropertyName("before")] object Before);
