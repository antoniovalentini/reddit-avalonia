using System.Text.Json;
using System.Text.Json.Serialization;

namespace Reddit.Client.Dtos.JsonConverters;

/// <summary>
/// JSON converter for deserializing the "edited" property from Reddit's Comment and User DTOs.
/// This property can be either a Unix epoch timestamp, indicating the last edit time, or <c>false</c> if the content was never edited.
/// </summary>
public class EditedJsonConverter : JsonConverter<long?>
{
    /// <summary>
    /// Reads and converts the JSON value to a nullable <see cref="long"/> representing the Unix epoch timestamp.
    /// If the JSON value is <c>false</c>, it returns <c>null</c>, indicating that the content was not edited.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The type to convert, expected to be <see cref="long"/>.</param>
    /// <param name="options">Serialization options to use.</param>
    /// <returns>A nullable <see cref="long"/> value representing the edit timestamp, or <c>null</c> if not edited.</returns>
    /// <exception cref="Exception">Thrown if the JSON value is not a number or <c>false</c>.</exception>
    public override long? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options)
    {
        var element = JsonElement.ParseValue(ref reader);
        return element.ValueKind switch
        {
            JsonValueKind.False => null,
            JsonValueKind.Number => (long)element.Deserialize<double>(),
            _ => throw new Exception($"Unable to parse JsonValueKind '{element.ValueKind}'")
        };
    }

    /// <summary>
    /// Writes a JSON representation of the edit timestamp.
    /// This method is not implemented as the converter is primarily intended for deserialization.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write to.</param>
    /// <param name="dateTimeValue">The nullable timestamp value to write.</param>
    /// <param name="options">Serialization options to use.</param>
    /// <exception cref="NotImplementedException">Thrown unconditionally as this converter does not support writing.</exception>
    public override void Write(
        Utf8JsonWriter writer,
        long? dateTimeValue,
        JsonSerializerOptions options) => throw new NotImplementedException();
}
