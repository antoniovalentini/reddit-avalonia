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
    protected readonly IRedditApiClient Client;
    protected readonly IAuthTokenStorage AuthTokenStorage;
    private readonly IMessenger _messenger;

    [ObservableProperty] private Post? _post;
    [ObservableProperty] private bool? _noComments = false;

    public ObservableCollection<CommentSimpleDto> Comments { get; set; } = [];

    public PostViewModel(Post post, IAuthTokenStorage authTokenStorage, IRedditApiClient client, IMessenger messenger)
    {
        AuthTokenStorage = authTokenStorage;
        Client = client;
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
        var authToken = AuthTokenStorage.Load();
        if (authToken is null || !authToken.IsValid) return;

        var response = await Client.CommentsSimple(Post.Subreddit, Post.Id, authToken);

        if (response.Length == 0)
        {
            NoComments = true;
            return;
        }
        Comments.Clear();
        Comments.AddRange(response);
    }
}
