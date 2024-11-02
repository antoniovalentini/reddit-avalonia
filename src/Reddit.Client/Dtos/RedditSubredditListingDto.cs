namespace Reddit.Client.Dtos;

public record RedditSubredditListingDto(string Kind, ListingData<KindData<Subreddit>> Data)
: KindData<ListingData<KindData<Subreddit>>>(Kind, Data);
