using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.FakeData;

namespace Reddilonia.ViewModels.Design;

public class MainViewModelDesign : MainViewModel
{
    public MainViewModelDesign() : base(
        new FakeRedditApiClient(),
        new FakeRedditAuthClient(),
        WeakReferenceMessenger.Default,
        new FakeAuthTokenStorage(),
        new FakeAuthManager(),
        new FakeLogger<MainViewModel>(),
        new FakeLogger<FeedsViewModel>(),
        new FakeLogger<SubRedditViewModel>())
    {
    }
}
