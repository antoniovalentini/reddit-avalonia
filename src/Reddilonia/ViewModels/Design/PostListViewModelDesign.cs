using CommunityToolkit.Mvvm.Messaging;
using Reddilonia.FakeData;

namespace Reddilonia.ViewModels.Design;

public class PostListViewModelDesign : PostListViewModel
{
    public PostListViewModelDesign() : base(FakeTools.FakePosts, WeakReferenceMessenger.Default, "u/Author_3") { }
}
