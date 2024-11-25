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
        IRedditAuthClient redditAuthClient,
        IRedditApiClient redditApiClient,
        IMessenger messenger,
        IAuthTokenStorage authTokenStorage,
        IAuthManager authManager,
        ILogger<FeedsViewModel> feedsLogger,
        ILogger<SubRedditViewModel> subRedditLogger)
    {
        feedsLogger.LogInformation("Initializing main view model...");

        messenger.Register<MainViewModel, LoadPostMessage>(this, (_, message) =>
        {
            CurrentPage = new PostViewModel(message.Post, redditAuthClient, redditApiClient, messenger);
        });
        messenger.Register<MainViewModel, ClosePostMessage>(this, (_, _) =>
        {
            CurrentPage = _feedsViewModel;
        });
        messenger.Register<MainViewModel, ReloadFeedsViewMessage>(this, (_, _) =>
        {
            _feedsViewModel = new FeedsViewModel(redditAuthClient, redditApiClient, messenger, authTokenStorage, feedsLogger, subRedditLogger, authManager);
            CurrentPage = _feedsViewModel;
        });

        _feedsViewModel = new FeedsViewModel(redditAuthClient, redditApiClient, messenger, authTokenStorage, feedsLogger, subRedditLogger, authManager);
        CurrentPage = _feedsViewModel;

        feedsLogger.LogInformation("Finished initializing main view model...");
    }
}
