using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.FakeData;

namespace Reddilonia.ViewModels.Design;

public class SubRedditViewModelDesign : SubRedditViewModel
{
    public SubRedditViewModelDesign()
        : base(
            FakeTools.FakeSubreddit,
            new FakeRedditApiClient(),
            new FakeAuthTokenStorage(),
            WeakReferenceMessenger.Default,
            new FakeLogger<SubRedditViewModel>())
    {

    }
}
