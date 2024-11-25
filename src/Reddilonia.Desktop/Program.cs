using System;
using Avalonia;
using Avalonia.WebView.Desktop;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Reddilonia.Desktop;

sealed class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<DesktopApp>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace()
            .UseDesktopWebView();
}

public class DesktopApp : App
{
    protected override void RegisterPlatformServices(IServiceCollection services) { }

    protected override void PlatformConfiguration(ConfigurationBuilder builder)
    {
        builder
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.test.json", true);
    }
}
