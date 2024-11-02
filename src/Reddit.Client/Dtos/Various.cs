using System.Text.Json.Serialization;

namespace Reddit.Client.Dtos;

public record Gildings;

public record Image(
    [property: JsonPropertyName("source")] Source Source,
    [property: JsonPropertyName("resolutions")] IReadOnlyList<Resolution> Resolutions,
    [property: JsonPropertyName("variants")] Variants Variants,
    [property: JsonPropertyName("id")] string Id
);

public record Media(
    [property: JsonPropertyName("reddit_video")] RedditVideo RedditVideo
);

public record MediaEmbed;

public record Preview(
    [property: JsonPropertyName("images")] IReadOnlyList<Image> Images,
    [property: JsonPropertyName("enabled")] bool? Enabled
);

public record RedditVideo(
    [property: JsonPropertyName("bitrate_kbps")] int? BitrateKbps,
    [property: JsonPropertyName("fallback_url")] string FallbackUrl,
    [property: JsonPropertyName("has_audio")] bool? HasAudio,
    [property: JsonPropertyName("height")] int? Height,
    [property: JsonPropertyName("width")] int? Width,
    [property: JsonPropertyName("scrubber_media_url")] string ScrubberMediaUrl,
    [property: JsonPropertyName("dash_url")] string DashUrl,
    [property: JsonPropertyName("duration")] int? Duration,
    [property: JsonPropertyName("hls_url")] string HlsUrl,
    [property: JsonPropertyName("is_gif")] bool? IsGif,
    [property: JsonPropertyName("transcoding_status")] string TranscodingStatus
);

public record Resolution(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("width")] int? Width,
    [property: JsonPropertyName("height")] int? Height
);


public record SecureMedia(
    [property: JsonPropertyName("reddit_video")] RedditVideo RedditVideo
);

public record SecureMediaEmbed;

public record Source(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("width")] int? Width,
    [property: JsonPropertyName("height")] int? Height
);

public record Variants;

public record CommentContributionSettings(
    [property: JsonPropertyName("allowed_media_types")] IReadOnlyList<string> AllowedMediaTypes
);

public record UserFlairRichtext(
    [property: JsonPropertyName("e")] string E,
    [property: JsonPropertyName("t")] string T
);
