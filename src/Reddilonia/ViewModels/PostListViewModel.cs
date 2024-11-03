using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.Models;
using Reddit.Client.Dtos;

namespace Reddilonia.ViewModels;

public partial class PostListViewModel : ViewModelBase
{
    private readonly IMessenger _messenger;
    public List<Post> Posts { get; set; }
    public string Op { get; set; }

    public PostListViewModel(List<Post> posts, IMessenger messenger, string op = "")
    {
        _messenger = messenger;
        Posts = posts;
        Op = op;
    }

    [ObservableProperty]
    private Post? _selectedPost;
    partial void OnSelectedPostChanged(Post? value)
    {
        if (value is null) return;

        _messenger.Send(new LoadPostMessage(value));
    }
}
