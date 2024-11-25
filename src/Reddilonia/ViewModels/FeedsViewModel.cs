using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Reddilonia.BusinessLogic;
using Reddit.Client;
using Reddit.Client.Dtos;

namespace Reddilonia.ViewModels;

public partial class FeedsViewModel : ViewModelBase
{
    private readonly IRedditApiClient _redditApiClient;
    private readonly IMessenger _messenger;
    private readonly ILogger<FeedsViewModel> _logger;
    private readonly ILogger<SubRedditViewModel> _subredditLogger;
    private readonly IAuthTokenStorage _authTokenStorage;
    private readonly IAuthManager _authManager;

    [ObservableProperty] private int _requestsTotal;
    [ObservableProperty] private int _requestsDone;
    [ObservableProperty] private bool _isPaneOpen;
    [ObservableProperty] private bool _needsAuthentication;
    [ObservableProperty] private bool _loading = true;
    [ObservableProperty] private bool _postLoaded;
    [ObservableProperty] private string _userName = "Unknown";

    [ObservableProperty] private ViewModelBase? _splitViewContent;

    public ObservableCollection<string> SubredditIds { get; set; } = [];
    private List<Subreddit> _subreddits = [];
    private List<Post> _homePosts = [];

    public FeedsViewModel(
        IRedditApiClient redditApiClient,
        IMessenger messenger,
        IAuthTokenStorage authTokenStorage,
        ILogger<FeedsViewModel> logger,
        ILogger<SubRedditViewModel> subredditLogger,
        IAuthManager authManager)
    {
        _redditApiClient = redditApiClient;
        _messenger = messenger;
        _authTokenStorage = authTokenStorage;
        _logger = logger;
        _subredditLogger = subredditLogger;
        _authManager = authManager;

        _redditApiClient.RateLimitUpdate += (_, args) =>
        {
            RequestsDone = args.Done;
            RequestsTotal = args.Total;
        };

        _ = LoadStuff();
    }

    private async Task LoadStuff()
    {
        var authToken = _authTokenStorage.Load();
        if (authToken is null || !authToken.IsValid)
        {
            _logger.LogWarning("Unable to find a valid access token");
            Loading = false;
            NeedsAuthentication = true;
            return;
        }
        _logger.LogInformation("VALID ACCESS TOKEN");

        // get user profile
        var user = await _redditApiClient.Me(authToken);
        UserName = $"Hello {user.Name}!";
        _logger.LogInformation("Username set");

        // load subreddits
        var subreddits = await _redditApiClient.Mine(authToken, "subscriber");
        _subreddits = subreddits.Data.Children.Where(c => c.Data.SubredditType == "public").Select(s => s.Data).ToList();
        SubredditIds.Clear();
        SubredditIds.AddRange(_subreddits.Select(s => s.DisplayNamePrefixed));

        // load best feeds
        var best = await _redditApiClient.Best(authToken);
        _homePosts = best.Data.Children.Select(x => x.Data).ToList();
        _logger.LogInformation("Retrieved best posts");

        SplitViewContent = new PostListViewModel(_homePosts, _messenger);
        Loading = false;
        _logger.LogInformation("Load complete");
    }

    [RelayCommand] private void TriggerPane()
    {
        IsPaneOpen = !IsPaneOpen;
    }

    [RelayCommand] private void BackHome()
    {
        SplitViewContent = new PostListViewModel(_homePosts, _messenger);
    }

    [ObservableProperty] private string? _selectedSubredditId;
    partial void OnSelectedSubredditIdChanged(string? value)
    {
        if (value is null) return;

        SplitViewContent = new SubRedditViewModel(_subreddits.First(s => s.DisplayNamePrefixed == value), _redditApiClient, _authTokenStorage, _messenger, _subredditLogger);
        IsPaneOpen = false;
    }

    [RelayCommand(CanExecute = nameof(CanShowAuth))]
    private void ShowAuth()
    {
        SplitViewContent = new AuthNavigationViewModel(_authManager, _authTokenStorage, _messenger);
        NeedsAuthentication = false;
    }

    private bool CanShowAuth()
    {
        var authToken = _authTokenStorage.Load();
        return authToken is null || !authToken.IsValid;
    }
}
