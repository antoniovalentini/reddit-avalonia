using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.FakeData;

namespace Reddilonia.ViewModels.Design;

public class PostViewModelDesign : PostViewModel
{
    public PostViewModelDesign() : base(Reddit.Client.Dtos.Post.Fake, new FakeAuthTokenStorage(), new FakeRedditApiClient(), WeakReferenceMessenger.Default) { }
}
