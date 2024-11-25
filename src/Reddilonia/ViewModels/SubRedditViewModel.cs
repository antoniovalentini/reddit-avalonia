using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Reddilonia.BusinessLogic;
using Reddit.Client;
using Reddit.Client.Dtos;

namespace Reddilonia.ViewModels;

public partial class SubRedditViewModel : ViewModelBase
{
    private readonly IRedditApiClient _apiClient;
    private readonly IMessenger _messenger;
    private readonly ILogger<SubRedditViewModel> _logger;
    private readonly IAuthTokenStorage _authTokenStorage;

    [ObservableProperty] private bool _loading = true;
    [ObservableProperty] private Subreddit _subreddit;
    [ObservableProperty] private ViewModelBase? _postsControl;

    public SubRedditViewModel(Subreddit subreddit, IRedditApiClient apiClient, IAuthTokenStorage authTokenStorage, IMessenger messenger, ILogger<SubRedditViewModel> logger)
    {
        _subreddit = subreddit;
        _apiClient = apiClient;
        _authTokenStorage = authTokenStorage;
        _messenger = messenger;
        _logger = logger;

        _ = LoadPosts();
    }

    private async Task LoadPosts()
    {
        var authToken = _authTokenStorage.Load();
        if (authToken is null || !authToken.IsValid)
        {
            _logger.LogWarning("Unable to find a valid access token");
            Loading = false;
            return;
        }

        var posts = await _apiClient.Hot(authToken, Subreddit.DisplayNamePrefixed);
        PostsControl = new PostListViewModel(posts.Data.Children.Select(x => x.Data).ToList(), _messenger);
        Loading = false;
    }
}
