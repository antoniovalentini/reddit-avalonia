using System;
using System.Collections.Generic;
using System.Text.Json;
using Reddit.Client.Dtos;

namespace Reddilonia.FakeData;

public class FakeTools
{
    public static double ToEpoch(DateTime dateTime)
        => (long)dateTime.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

    private static Post DefaultPost => JsonSerializer.Deserialize<Post>("{}")!;

    private static Post FakePost(string title, string author, string subreddit, DateTime datetime, string imageUrl = "") =>
        DefaultPost with
        {
            Title = title, Author = author, SubredditNamePrefixed = subreddit, CreatedUtc = ToEpoch(datetime), Thumbnail = imageUrl,
            NumComments = 42, Score = 999,
            Selftext = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Etiam pellentesque pretium porta. Donec in molestie dolor, ac ullamcorper sapien. " +
                       "Integer euismod tincidunt diam. Nunc suscipit vel metus vitae mattis. Aliquam erat volutpat. Nam nisl enim, placerat vitae nisi quis, tincidunt auctor turpis. " +
                       "Quisque a sapien porttitor turpis condimentum facilisis et efficitur tellus.",
        };

    public static readonly List<Post> FakePosts =
    [
        FakePost("Post 1 with a very very very very very very very very very very very very very very very very very very very very very very very very long title", "Author_1", "r/subreddit_1", DateTime.UtcNow.AddSeconds(-2)),
        FakePost("Post 2 with a tiny title", "Author_2", "r/subreddit_2", DateTime.UtcNow.AddMinutes(-2)),
        FakePost("Post 3 with a tiny title", "Author_3", "r/subreddit_3", DateTime.UtcNow.AddHours(-2)),
        FakePost("Post 4 with a tiny title", "Author_4", "r/subreddit_4", DateTime.UtcNow.AddDays(-2), "https://images.unsplash.com/photo-1607956853617-d9d248a8f327?q=80&w=600&auto=format&fit=crop"),
        FakePost("Post 5 with a very very very very very very very very very very very very very very very very very very very very very very very very long title", "Author_5", "r/subreddit_5", DateTime.UtcNow.AddDays(-15)),
        FakePost("Post 6 with a tiny title", "Author_6", "r/subreddit_6", DateTime.UtcNow.AddDays(-57)),
        FakePost("Post 7 with a tiny title", "Author_7", "r/subreddit_7", DateTime.UtcNow.AddYears(-2)),
    ];

    public static Subreddit FakeSubreddit
    {
        get
        {
            var subreddit = JsonSerializer.Deserialize<Subreddit>("{}");
            return subreddit! with
            {
                Url = "/r/italy",
                Subscribers = 1000,
                ActiveUserCount = 150,
                PublicDescription =
                "A subreddit for Italy aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa aaaaaaaaaa aaaaaaaaaaaaa aaaaaaaaaaaaaa aaaaaaaaaaa aaaaaaaaaaa aaaaaaaaaaa aaaaaaaaaaa aaaaaaaaaaaaa\n" +
                "some more stuff\n\n" +
                "even mooooore stuff\n\n" +
                "what else.....\n\n" +
                "oh yes, more stuff!",
            };
        }
    }
}
