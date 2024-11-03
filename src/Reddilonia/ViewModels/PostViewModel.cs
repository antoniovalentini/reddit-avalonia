using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.Models;
using Reddit.Client;
using Reddit.Client.Dtos;

namespace Reddilonia.ViewModels;

public partial class PostViewModel : ViewModelBase
{
    private readonly IRedditAuthClient _authClient;
    private readonly IRedditApiClient _client;
    private readonly IMessenger _messenger;

    [ObservableProperty] private Post? _post;
    [ObservableProperty] private bool? _noComments = false;

    public ObservableCollection<CommentSimpleDto> Comments { get; set; } = [];

    public PostViewModel(Post post, IRedditAuthClient authClient, IRedditApiClient client, IMessenger messenger)
    {
        _authClient = authClient;
        _client = client;
        _messenger = messenger;
        Post = post;

        _ = LoadComments();
    }

    [RelayCommand]
    private void ClosePost()
    {
        _messenger.Send(new ClosePostMessage());
    }

    private async Task LoadComments()
    {
        if (Post is null) return;
        if (_authClient.CurrentOAuthToken is null) return;
        var response = await _client.CommentsSimple(Post.Subreddit, Post.Id, _authClient.CurrentOAuthToken);

        if (response.Length == 0)
        {
            NoComments = true;
            return;
        }
        Comments.Clear();
        Comments.AddRange(response);
    }
}
