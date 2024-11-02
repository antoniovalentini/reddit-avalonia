using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reddit.Client.Dtos.JsonConverters;

/// <summary>
/// JSON converter for deserializing the "replies" property from Reddit's API response.
/// The "replies" property can either be a deserializable JSON object or a string (possibly empty or "false") when no replies are present.
/// This converter returns an empty <see cref="Reddit.Client.Dtos.KindData{T}"/> instance in case of a string value.
/// </summary>
public class RepliesJsonConverter : JsonConverter<KindData<ListingData<KindData<CommentDto>>>>
{
    /// <summary>
    /// Reads and converts the JSON value of the "replies" property to a <see cref="Reddit.Client.Dtos.KindData{T}"/> object.
    /// If the JSON value is a string, an empty instance is returned, as Reddit sometimes represents "no replies" with a string (probably).
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert, expected to be KindData{ListingData{KindData{CommentDto}}}.</param>
    /// <param name="options">Serialization options to use.</param>
    /// <returns>A KindData{ListingData{KindData{CommentDto}}} object, either populated from JSON data or empty if no replies are present.</returns>
    /// <exception cref="Exception">Thrown if the JSON value cannot be parsed as a valid "replies" object or string.</exception>
    public override KindData<ListingData<KindData<CommentDto>>> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var element = JsonElement.ParseValue(ref reader);
        if (element.ValueKind == JsonValueKind.String)
        {
            return new KindData<ListingData<KindData<CommentDto>>>("Listing",
                new ListingData<KindData<CommentDto>>("", -1, null!, "", new List<KindData<CommentDto>>(), null!));

        }

        if (element.ValueKind == JsonValueKind.Object)
        {
            var kindData = element.Deserialize<KindData<ListingData<KindData<CommentDto>>>>();
            return kindData ?? throw new Exception("Unable to deserialize replies");
        }

        throw new Exception($"Unable to parse JsonValueKind '{element.ValueKind}'");
    }

    /// <summary>
    /// Writes a JSON representation of the "replies" data.
    /// This method is not implemented as the converter is intended primarily for deserialization.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="dateTimeValue">The "replies" data to write.</param>
    /// <param name="options">Serialization options to use.</param>
    /// <exception cref="NotImplementedException">Thrown unconditionally as this converter does not support writing.</exception>
    public override void Write(
        Utf8JsonWriter writer,
        KindData<ListingData<KindData<CommentDto>>> dateTimeValue,
        JsonSerializerOptions options) => throw new NotImplementedException();
}
