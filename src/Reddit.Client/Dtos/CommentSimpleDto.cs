namespace Reddit.Client.Dtos;

public record CommentSimpleDto(
    string Body,
    bool Stickied,
    int Score,
    string Author,
    bool IsOp,
    double? CreatedUtc,
    IReadOnlyList<CommentSimpleDto> Replies)
{
    public string TimeSpanFromCreationEpoch => CreatedUtc.TimeSpanFromCreationEpoch();
}
