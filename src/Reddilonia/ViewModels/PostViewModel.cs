using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.BusinessLogic;
using Reddilonia.Models;
using Reddit.Client;
using Reddit.Client.Dtos;

namespace Reddilonia.ViewModels;

public partial class PostViewModel : ViewModelBase
{
    private readonly IRedditApiClient _client;
    private readonly IMessenger _messenger;
    private readonly IAuthTokenStorage _authTokenStorage;

    [ObservableProperty] private Post? _post;
    [ObservableProperty] private bool? _noComments = false;

    public ObservableCollection<CommentSimpleDto> Comments { get; set; } = [];

    public PostViewModel(Post post, IAuthTokenStorage authTokenStorage, IRedditApiClient client, IMessenger messenger)
    {
        _authTokenStorage = authTokenStorage;
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
        var authToken = _authTokenStorage.Load();
        if (authToken is null || !authToken.IsValid) return;

        var response = await _client.CommentsSimple(Post.Subreddit, Post.Id, authToken);

        if (response.Length == 0)
        {
            NoComments = true;
            return;
        }
        Comments.Clear();
        Comments.AddRange(response);
    }
}
