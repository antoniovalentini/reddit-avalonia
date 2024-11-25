using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.FakeData;

namespace Reddilonia.ViewModels.Design;

public class PostViewModelDesign : PostViewModel
{
    public PostViewModelDesign() : base(Reddit.Client.Dtos.Post.Fake, new FakeAuthTokenStorage(),
        new FakeRedditApiClient(), WeakReferenceMessenger.Default)
    {
        var authToken = AuthTokenStorage.Load();
        var response = Client.CommentsSimple(Post!.Subreddit, Post.Id, authToken!)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

        Comments.Clear();
        Comments.AddRange(response);
    }
}
