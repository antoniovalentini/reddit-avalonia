using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Logging;
using Reddit.Client;
using Reddit.Client.Dtos;

namespace Reddilonia.ViewModels;

public partial class SubRedditViewModel : ViewModelBase
{
    private readonly IRedditApiClient _apiClient;
    private readonly IRedditAuthClient _authClient;
    private readonly IMessenger _messenger;
    private readonly ILogger<SubRedditViewModel> _logger;

    [ObservableProperty] private bool _loading = true;
    [ObservableProperty] private Subreddit _subreddit;
    [ObservableProperty] private ViewModelBase? _postsControl;

    public SubRedditViewModel(Subreddit subreddit, IRedditApiClient apiClient, IRedditAuthClient authClient, IMessenger messenger, ILogger<SubRedditViewModel> logger)
    {
        _subreddit = subreddit;
        _apiClient = apiClient;
        _authClient = authClient;
        _messenger = messenger;
        _logger = logger;

        _ = LoadPosts();
    }

    private async Task LoadPosts()
    {
        if (_authClient.CurrentOAuthToken is null)
        {
            _logger.LogWarning("CurrentOAuthToken is null");
            Loading = false;
            return;
        }

        var posts = await _apiClient.Hot(_authClient.CurrentOAuthToken, Subreddit.DisplayNamePrefixed);
        PostsControl = new PostListViewModel(posts.Data.Children.Select(x => x.Data).ToList(), _messenger);
        Loading = false;
    }
}
