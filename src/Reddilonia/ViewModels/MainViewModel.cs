using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Reddilonia.BusinessLogic;
using Reddilonia.Models;
using Reddit.Client;

namespace Reddilonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IRedditApiClient _redditApiClient;
    private readonly IRedditAuthClient _redditAuthClient;
    private readonly IMessenger _messenger;
    private readonly IAuthTokenStorage _authTokenStorage;
    private readonly IAuthManager _authManager;
    private readonly ILogger<MainViewModel> _logger;
    private readonly ILogger<FeedsViewModel> _feedsLogger;
    private readonly ILogger<SubRedditViewModel> _subRedditLogger;
    private FeedsViewModel? _feedsViewModel;

    [ObservableProperty] private ViewModelBase? _currentPage;
    [ObservableProperty] private bool _needsAuthentication;
    [ObservableProperty] private bool _loading = true;


    public MainViewModel(
        IRedditApiClient redditApiClient,
        IRedditAuthClient redditAuthClient,
        IMessenger messenger,
        IAuthTokenStorage authTokenStorage,
        IAuthManager authManager,
        ILogger<MainViewModel> logger,
        ILogger<FeedsViewModel> feedsLogger,
        ILogger<SubRedditViewModel> subRedditLogger)
    {
        _redditApiClient = redditApiClient;
        _redditAuthClient = redditAuthClient;
        _messenger = messenger;
        _authTokenStorage = authTokenStorage;
        _authManager = authManager;
        _logger = logger;
        _feedsLogger = feedsLogger;
        _subRedditLogger = subRedditLogger;

        messenger.Register<MainViewModel, LoadPostMessage>(this, (_, message) =>
        {
            CurrentPage = new PostViewModel(message.Post, authTokenStorage, redditApiClient, messenger);
        });
        messenger.Register<MainViewModel, ClosePostMessage>(this, (_, _) =>
        {
            CurrentPage = _feedsViewModel;
        });
        messenger.Register<MainViewModel, ReloadFeedsViewMessage>(this, (_, _) =>
        {
            _feedsViewModel = new FeedsViewModel(redditApiClient, messenger, authTokenStorage, feedsLogger, subRedditLogger);
            CurrentPage = _feedsViewModel;
        });

        _ = Init();
    }

    private async Task Init()
    {
        var authToken = _authTokenStorage.Load();
        if (authToken is null)
        {
            _logger.LogWarning("No access token found");
            Loading = false;
            NeedsAuthentication = true;
            return;
        }
        if (!authToken.IsValid)
        {
            try
            {
                _logger.LogInformation("Refreshing auth token");
                authToken = await _redditAuthClient.RefreshToken(authToken.RefreshToken);
                await _authTokenStorage.StoreToken(authToken);
            }
            catch (Exception e)
            {
                _logger.LogWarning("Unable to retrieve an auth access token: {Error}", e.Message);
                Loading = false;
                NeedsAuthentication = true;
                return;
            }
        }

        _logger.LogInformation("VALID ACCESS TOKEN");

        _feedsViewModel = new FeedsViewModel(_redditApiClient, _messenger, _authTokenStorage, _feedsLogger, _subRedditLogger);
        CurrentPage = _feedsViewModel;
        NeedsAuthentication = false;
        Loading = false;
    }

    [RelayCommand]
    private void ShowAuth()
    {
        CurrentPage = new AuthNavigationViewModel(_authManager, _authTokenStorage, _messenger);
        NeedsAuthentication = false;
    }
}
