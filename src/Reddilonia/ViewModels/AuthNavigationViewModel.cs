using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.BusinessLogic;
using Reddilonia.Models;
using Reddit.Client.Dtos;

namespace Reddilonia.ViewModels;

public partial class AuthNavigationViewModel : ViewModelBase
{
    private readonly IAuthManager _authManager;
    private readonly IAuthTokenStorage _authTokenStorage;
    private readonly IMessenger _messenger;

    [ObservableProperty] private Uri _webViewUri = new("https://www.google.com");

    public AuthNavigationViewModel(IAuthManager authManager, IAuthTokenStorage authTokenStorage, IMessenger messenger)
    {
        _authManager = authManager;
        _authTokenStorage = authTokenStorage;
        _messenger = messenger;
        _authManager.AuthSuccess += AuthSuccess;
        _authManager.Start();
        WebViewUri = new Uri(_authManager.GetAuthUrl());
    }

    private async void AuthSuccess(object? sender, AuthSuccessEventArgs e)
    {
        var token = new OAuthToken(e.AccessToken, "bearer", 86400, "*", e.RefreshToken);
        await _authTokenStorage.StoreToken(token);
        _authManager.Stop();
        _messenger.Send<ReloadFeedsViewMessage>();
    }
}
