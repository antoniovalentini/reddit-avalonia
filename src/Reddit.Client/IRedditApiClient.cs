using Reddit.Client.Dtos;

namespace Reddit.Client;

public record RateLimitUpdateEventArgs(int Done, int Total, int Reset);

/// <summary>
/// Interface for interacting with Reddit's API endpoints, accessed at <see href="https://oauth.reddit.com/">https://oauth.reddit.com/</see>.
/// All methods require an OAuth token for authentication.
/// </summary>
public interface IRedditApiClient
{
    /// <summary>
    /// Event triggered when there is an update to the API rate limit status.
    /// </summary>
    event EventHandler<RateLimitUpdateEventArgs>? RateLimitUpdate;


    /// <summary>
    /// Retrieves a list of best posts from the user's subreddits.
    /// </summary>
    /// <param name="authToken">OAuth token required for authentication.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of best posts.</returns>
    Task<ListingBestDto> Best(OAuthToken authToken);

    /// <summary>
    /// Retrieves a list of hot posts. Optionally, a specific subreddit can be targeted.
    /// </summary>
    /// <param name="authToken">OAuth token required for authentication.</param>
    /// <param name="subredditPrefixed">The prefixed name of the subreddit (e.g., "r/csharp"). If empty, retrieves hot posts from the user's subreddits.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of hot posts.</returns>
    Task<ListingBestDto> Hot(OAuthToken authToken, string subredditPrefixed = "");

    /// <summary>
    /// Retrieves a list of new posts. Optionally, a specific subreddit can be targeted.
    /// </summary>
    /// <param name="authToken">OAuth token required for authentication.</param>
    /// <param name="subredditPrefixed">The prefixed name of the subreddit (e.g., "r/csharp"). If empty, retrieves new posts from the user's subreddits.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of new posts.</returns>
    Task<ListingBestDto> New(OAuthToken authToken, string subredditPrefixed = "");

    /// <summary>
    /// Retrieves information about the authenticated Reddit user.
    /// </summary>
    /// <param name="authToken">OAuth token required for authentication.</param>
    /// <returns>A task that represents the asynchronous operation, containing the user's information.</returns>
    Task<RedditUser> Me(OAuthToken authToken);

    /// <summary>
    /// Retrieves a list of subreddits associated with the authenticated user, such as subscribed or created subreddits.
    /// </summary>
    /// <param name="authToken">OAuth token required for authentication.</param>
    /// <param name="subredditType">The type of subreddits to retrieve (e.g., "subscriber" for subscribed subreddits). If empty, retrieves all.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of subreddits.</returns>
    Task<RedditSubredditListingDto> Mine(OAuthToken authToken, string? subredditType = "");

    /// <summary>
    /// Retrieves a detailed listing of comments for a specific article in a subreddit.
    /// </summary>
    /// <param name="subreddit">The name of the subreddit (e.g., "csharp").</param>
    /// <param name="articleId">The ID of the article (post) to retrieve comments for.</param>
    /// <param name="authToken">OAuth token required for authentication.</param>
    /// <returns>A task that represents the asynchronous operation, containing the list of comments for the specified article.</returns>
    Task<KindData<ListingData<KindData<CommentDto>>>[]> Comments(string subreddit, string articleId, OAuthToken authToken);

    /// <summary>
    /// Retrieves a simplified list of comments for a specific article in a subreddit.
    /// </summary>
    /// <param name="subreddit">The name of the subreddit (e.g., "csharp").</param>
    /// <param name="articleId">The ID of the article (post) to retrieve comments for.</param>
    /// <param name="authToken">OAuth token required for authentication.</param>
    /// <returns>A task that represents the asynchronous operation, containing a simplified list of comments for the specified article.</returns>
    Task<CommentSimpleDto[]> CommentsSimple(string subreddit, string articleId, OAuthToken authToken);
}
