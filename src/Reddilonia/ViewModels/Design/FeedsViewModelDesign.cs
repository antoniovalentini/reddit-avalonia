using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.FakeData;

namespace Reddilonia.ViewModels.Design;

public class FeedsViewModelDesign : FeedsViewModel
{
    public FeedsViewModelDesign() : base(
        new FakeRedditApiClient(),
        WeakReferenceMessenger.Default,
        new FakeAuthTokenStorage(),
        new FakeLogger<FeedsViewModel>(),
        new FakeLogger<SubRedditViewModel>(),
        new FakeAuthManager())
    {
        RequestsTotal = 100;
        RequestsDone = 25;
    }
}
