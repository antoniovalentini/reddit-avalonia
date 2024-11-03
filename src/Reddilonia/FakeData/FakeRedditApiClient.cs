using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Reddit.Client;
using Reddit.Client.Dtos;

namespace Reddilonia.FakeData;

public class FakeRedditApiClient : IRedditApiClient
{
    public event EventHandler<RateLimitUpdateEventArgs>? RateLimitUpdate;

    public Task<ListingBestDto> Best(OAuthToken authToken)
    {
        var post = JsonSerializer.Deserialize<Post>("{}");
        post = post! with { CreatedUtc = FakeTools.ToEpoch(DateTime.UtcNow) };
        var posts = Enumerable
            .Range(1, 8)
            .Select(i => new KindData<Post>("tx", post with
            {
                SubredditNamePrefixed = $"r/subreddit-{i}",
                Title = $"title-{i}",
                Author = $"author-{i}",
                Thumbnail = i == 4 ? "https://images.unsplash.com/photo-1607956853617-d9d248a8f327?q=80&w=600&auto=format&fit=crop" : "",
                Selftext = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam pellentesque pretium porta. Donec in molestie dolor, ac ullamcorper sapien. Integer euismod tincidunt diam. Nunc suscipit vel metus vitae mattis. Aliquam erat volutpat. Nam nisl enim, placerat vitae nisi quis, tincidunt auctor turpis. Quisque a sapien porttitor turpis condimentum facilisis et efficitur tellus. Phasellus ut facilisis nibh, a feugiat lacus. Quisque id vehicula purus. Phasellus dapibus leo nec augue sagittis, sit amet vestibulum est imperdiet. Donec pretium in nulla a vulputate. Donec ultrices condimentum sem, sit amet elementum libero aliquet eu. Nullam sit amet maximus lacus."
            }));
        var kindData = new List<KindData<Post>>(posts);
        return Task.FromResult(new ListingBestDto("test", new ListingData<KindData<Post>>("", 0, 1, "", kindData, 1)));
    }

    public Task<ListingBestDto> Hot(OAuthToken authToken, string subredditPrefixed = "") => Best(authToken);

    public Task<ListingBestDto> New(OAuthToken authToken, string subredditPrefixed = "") => Best(authToken);

    public Task<RedditUser> Me(OAuthToken authToken)
    {
        var user = JsonSerializer.Deserialize<RedditUser>("{}");
        return Task.FromResult(user! with { Name = "test-design" });
    }

    public Task<RedditSubredditListingDto> Mine(OAuthToken authToken, string? subredditType = "")
    {
        var subreddit = JsonSerializer.Deserialize<Subreddit>("{}");
        subreddit = subreddit! with { SubredditType = "public" };
        var subreddits = new List<KindData<Subreddit>>
        {
            new("t3", subreddit with {DisplayNamePrefixed = "r/subreddit-1"}),
            new("t3", subreddit with {DisplayNamePrefixed = "r/subreddit-2"}),
            new("t3", subreddit with {DisplayNamePrefixed = "r/subreddit-3"}),
        };
        return Task.FromResult(new RedditSubredditListingDto("Listing", new ListingData<KindData<Subreddit>>("", 0, 1, "", subreddits, 1)));
    }

    public Task<KindData<ListingData<KindData<CommentDto>>>[]> Comments(string subreddit, string articleId, OAuthToken authToken)
    {
        return Task.FromResult(new[]
        {
            new KindData<ListingData<KindData<CommentDto>>>("Listing", new ListingData<KindData<CommentDto>>("", 0, "null", "", new List<KindData<CommentDto>>(), "null")),
            new KindData<ListingData<KindData<CommentDto>>>("Listing", new ListingData<KindData<CommentDto>>("", 0, "null", "", new List<KindData<CommentDto>>(), "null")),
        });
    }

    public Task<CommentSimpleDto[]> CommentsSimple(string subreddit, string articleId, OAuthToken authToken)
    {
        return Task.FromResult(new List<CommentSimpleDto>
        {
            new("Lorem ipsum 1", false, 10, "Author_1", false, FakeTools.ToEpoch(DateTime.UtcNow.AddSeconds(-5)), new List<CommentSimpleDto>()),
            new("Lorem ipsum 2", false, 20, "Author_2", true, FakeTools.ToEpoch(DateTime.UtcNow.AddDays(-1)), new List<CommentSimpleDto>{ new("Lorem ipsum 2.1", false, 4, "Author_1", false, FakeTools.ToEpoch(DateTime.UtcNow.AddDays(-2)), new List<CommentSimpleDto>()) }),
            new("Lorem ipsum 3", false, 9, "Author_3", false, FakeTools.ToEpoch(DateTime.UtcNow.AddDays(-20)), new List<CommentSimpleDto>()),
            new("Lorem ipsum 4", true, 1, "Author_4", false, FakeTools.ToEpoch(DateTime.UtcNow.AddDays(-5)), new List<CommentSimpleDto>{ new("Lorem ipsum 4.1", false, 15, "Author_1", false, FakeTools.ToEpoch(DateTime.UtcNow.AddDays(-30)), new List<CommentSimpleDto>{ new("Lorem ipsum 4.1.1", false, 2, "Author_4", false, FakeTools.ToEpoch(DateTime.UtcNow.AddDays(-4)), new List<CommentSimpleDto>()) }) }),
        }.ToArray());
    }
}
