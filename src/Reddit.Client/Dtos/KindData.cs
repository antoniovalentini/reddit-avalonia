using System.Text.Json.Serialization;

namespace Reddit.Client.Dtos;

public record KindData<T>(
    [property: JsonPropertyName("kind")] string Kind,
    [property: JsonPropertyName("data")] T Data
);
