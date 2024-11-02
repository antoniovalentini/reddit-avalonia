namespace Reddit.Client.Dtos;

public record ListingBestDto(string Kind, ListingData<KindData<Post>> Data)
    : KindData<ListingData<KindData<Post>>>(Kind, Data);
