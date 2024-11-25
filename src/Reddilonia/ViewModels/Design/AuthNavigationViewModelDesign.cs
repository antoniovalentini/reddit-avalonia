using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.FakeData;

namespace Reddilonia.ViewModels.Design;

public class AuthNavigationViewModelDesign : AuthNavigationViewModel
{
    public AuthNavigationViewModelDesign() : base(new FakeAuthManager(), new FakeAuthTokenStorage(), WeakReferenceMessenger.Default)
    {
    }
}
