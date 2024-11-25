using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Reddilonia.BusinessLogic;
using Reddilonia.Models;
using Reddit.Client;

namespace Reddilonia.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private ViewModelBase? _currentPage;

    private FeedsViewModel? _feedsViewModel;

    public MainViewModel(
        IRedditApiClient redditApiClient,
        IRedditAuthClient redditAuthClient,
        IMessenger messenger,
        IAuthTokenStorage authTokenStorage,
        IAuthManager authManager,
        ILogger<FeedsViewModel> feedsLogger,
        ILogger<SubRedditViewModel> subRedditLogger)
    {
        feedsLogger.LogInformation("Initializing main view model...");

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
            _feedsViewModel = new FeedsViewModel(redditApiClient, redditAuthClient, messenger, authTokenStorage, feedsLogger, subRedditLogger, authManager);
            CurrentPage = _feedsViewModel;
        });

        _feedsViewModel = new FeedsViewModel(redditApiClient, redditAuthClient, messenger, authTokenStorage, feedsLogger, subRedditLogger, authManager);
        CurrentPage = _feedsViewModel;

        feedsLogger.LogInformation("Finished initializing main view model...");
    }
}
