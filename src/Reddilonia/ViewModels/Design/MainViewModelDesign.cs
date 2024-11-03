using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.FakeData;

namespace Reddilonia.ViewModels.Design;

public class MainViewModelDesign : MainViewModel
{
    public MainViewModelDesign() : base(
        new FakeRedditAuthClient(),
        new FakeRedditApiClient(),
        WeakReferenceMessenger.Default,
        new FakeAuthTokenStorage(),
        new FakeLogger<FeedsViewModel>(),
        new FakeLogger<SubRedditViewModel>())
    {
    }
}
