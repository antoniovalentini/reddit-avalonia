using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Reddilonia.ViewModels;
using Reddilonia.Views;
using Reddit.Client;

namespace Reddilonia;

public abstract class App : Application
{
    public static App Instance => (Current as App)!;
    public ServiceProvider? ServiceProvider { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    // These methods will be implemented in platform-specific projects
    protected abstract void RegisterPlatformServices(IServiceCollection services);
    protected abstract void PlatformConfiguration(ConfigurationBuilder builder);

    public override void OnFrameworkInitializationCompleted()
    {
        if (Design.IsDesignMode)
        {
            base.OnFrameworkInitializationCompleted();
            return;
        }

        var configBuilder = new ConfigurationBuilder();

        PlatformConfiguration(configBuilder);

        var config = configBuilder.Build();

        var services = new ServiceCollection()
            .AddLogging(builder => builder.AddConsole())
            .Configure<RedditClientSettings>(config)
            .AddSingleton<IMessenger>(WeakReferenceMessenger.Default)
            .AddSingleton<BitmapAssetValueConverter>()
            .AddSingleton<MainViewModel>();

        services.AddHttpClient<IRedditAuthClient, RedditAuthClient>(client =>
        {
            client.BaseAddress = new Uri("https://www.reddit.com/");
            client.DefaultRequestHeaders.Add("User-Agent", "RedditClient/0.1");
        });
        services.AddHttpClient<IRedditApiClient, RedditApiClient>(client =>
        {
            client.BaseAddress = new Uri("https://oauth.reddit.com/");
            client.DefaultRequestHeaders.Add("User-Agent", "RedditClient/0.1");
        });
        services.AddHttpClient<MemoryStreamWebRetriever>(client =>
        {
            client.DefaultRequestHeaders.Add("User-Agent", "RedditClient/0.1");
        });

        RegisterPlatformServices(services);

        ServiceProvider = services.BuildServiceProvider();
        var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();

        switch (ApplicationLifetime)
        {
            case IClassicDesktopStyleApplicationLifetime desktop:
                desktop.MainWindow = new MainWindow
                {
                    DataContext = mainViewModel,
                };
                break;
            case ISingleViewApplicationLifetime singleViewPlatform:
                singleViewPlatform.MainView = new MainView
                {
                    DataContext = mainViewModel,
                };
                break;
        }

        base.OnFrameworkInitializationCompleted();
    }
}
