using Avalonia.Controls;

namespace Reddilonia.Views;

public partial class AuthNavigationView : UserControl
{
    public AuthNavigationView()
    {
        InitializeComponent();
        PartWebView.WebViewNewWindowRequested += PART_WebView_WebViewNewWindowRequested;
    }

    private void PART_WebView_WebViewNewWindowRequested(object? sender, WebViewCore.Events.WebViewNewWindowEventArgs e)
    {
        e.UrlLoadingStrategy = WebViewCore.Enums.UrlRequestStrategy.OpenInNewWindow;
    }
}
