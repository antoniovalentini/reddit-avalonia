using System.Globalization;
using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Reddit.Client.Dtos;

namespace Reddit.Client;

public class RedditApiClient : IRedditApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RedditApiClient> _logger;
    private const string DefaultUserAgent = "RedditClient/1.0";

    public RedditApiClient(HttpClient httpClient, ILogger<RedditApiClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;

        if (_httpClient.DefaultRequestHeaders.UserAgent.Count == 0)
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
        }
    }

    private string AppendSlash(string url)
    {
        if (string.IsNullOrWhiteSpace(url)) return string.Empty;
        return url.StartsWith('/') ? url : $"/{url}";
    }

    public event EventHandler<RateLimitUpdateEventArgs>? RateLimitUpdate;

    public async Task<ListingBestDto> Best(OAuthToken authToken) => await Get<ListingBestDto>("/best", authToken);

    public async Task<ListingBestDto> Hot(OAuthToken authToken, string subredditPrefixed = "") => await Get<ListingBestDto>($"{AppendSlash(subredditPrefixed)}/hot", authToken);

    public async Task<ListingBestDto> New(OAuthToken authToken, string subredditPrefixed = "") => await Get<ListingBestDto>($"{AppendSlash(subredditPrefixed)}/new", authToken);

    public async Task<RedditUser> Me(OAuthToken authToken) => await Get<RedditUser>("/api/v1/me", authToken);

    public async Task<RedditSubredditListingDto> Mine(OAuthToken authToken, string? subredditType = "subscriber")
        => await Get<RedditSubredditListingDto>($"/subreddits/mine/{subredditType}", authToken);

    public async Task<KindData<ListingData<KindData<CommentDto>>>[]> Comments(string subreddit, string articleId, OAuthToken authToken)
        => await Get<KindData<ListingData<KindData<CommentDto>>>[]>($"r/{subreddit}/comments/{articleId}?sort=top", authToken);

    public async Task<CommentSimpleDto[]> CommentsSimple(string subreddit, string articleId, OAuthToken authToken)
    {
        var kindData = await Get<KindData<ListingData<KindData<CommentDto>>>[]>($"r/{subreddit}/comments/{articleId}?sort=top", authToken);
        // TODO: it's also possible to receive a t3 (link)
        var comments = kindData.FirstOrDefault(t => t.Kind == "Listing" && t.Data.Children.Any(c => c.Kind == "t1"));
        if (comments == null) return [];
        var children = comments.Data.Children.Select(c => c.Data).ToList();
        return SimplifyComments(children).ToArray();
    }

    private List<CommentSimpleDto> SimplifyComments(List<CommentDto> from)
    {
        return from.Select(SimplifyComment).ToList();
    }

    private CommentSimpleDto SimplifyComment(CommentDto from)
    {
        List<CommentSimpleDto> replies;
        if (from.Replies is not null && from.Replies.Data.Children.Any(c => c.Kind == "t1"))
        {
            try
            {
                // TODO: handle children with "more"
                replies = from.Replies.Data.Children.Where(c => c.Kind != "more").Select(c => SimplifyComment(c.Data)).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Unable to simplify comments");
                replies = [];
            }
        }
        else
        {
            if (from.Replies is null) _logger.LogError("Unrecognized comment: {From}", from);
            replies = [];
        }
        return new CommentSimpleDto(from.Body, from.Stickied, from.Score, from.Author, from.IsSubmitter, from.CreatedUtc, replies);
    }

    private async Task<T> Get<T>(string url, OAuthToken authToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", authToken.AccessToken);
        var response = await _httpClient.SendAsync(request);
        var (done, total, reset) = GetRateLimit(response.Headers);
        RateLimitUpdate?.Invoke(this, new RateLimitUpdateEventArgs(done, total, reset));
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Web request failed: {Error}", responseContent);
            throw new Exception("Failed to send request: " + responseContent);
        }

        T? result;
        try
        {
            result = JsonSerializer.Deserialize<T>(responseContent);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to deserialized response content: {Content}", responseContent);
            throw;
        }

        if (result is null)
        {
            _logger.LogError("Deserialized response is null: {Content}", responseContent);
            throw new Exception("Deserialized response: " + responseContent);
        }
        return result;
    }

    private (int, int, int) GetRateLimit(HttpResponseHeaders headers)
    {
        int done = 0, total = 0, reset = 0;
        if (headers.TryGetValues("x-ratelimit-used", out var doneString))
        {
            int.TryParse(doneString.FirstOrDefault(), out done);
        }
        if (headers.TryGetValues("x-ratelimit-remaining", out var remainingString) &&
            decimal.TryParse(remainingString.FirstOrDefault(), CultureInfo.InvariantCulture, out var remaining))
        {
            total = done + (int)remaining;
        }
        if (headers.TryGetValues("x-ratelimit-reset", out var resetString))
        {
            int.TryParse(resetString.FirstOrDefault(), out reset);
        }
        return (done, total, reset);
    }
}
