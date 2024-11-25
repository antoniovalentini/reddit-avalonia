using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Android.App;
using Android.Content.PM;
using Avalonia;
using Avalonia.Android;
using Avalonia.WebView.Android;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Reddilonia.Android;

[Activity(
    Label = "Reddilonia.Android",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize | ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<AndroidApp>
{
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .UseAndroidWebView();
    }
}

public class AndroidApp : App
{
    protected override void RegisterPlatformServices(IServiceCollection services) { }

    protected override void PlatformConfiguration(ConfigurationBuilder builder)
    {
        var manifestResourceNames = Assembly
            .GetExecutingAssembly()
            .GetManifestResourceNames();

        // PROD settings
        var configFilename = "appsettings.json";
        var appsettingsResName = manifestResourceNames
            .FirstOrDefault(r => r.EndsWith(configFilename, StringComparison.OrdinalIgnoreCase));
        if (appsettingsResName is null)
        {
            throw new FileNotFoundException($" The configuration file '{configFilename}' was not found and is not optional.");
        }
        var resourceStream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream(appsettingsResName);
        ArgumentNullException.ThrowIfNull(resourceStream);
        builder.AddJsonStream(resourceStream);

        // TEST settings (optional)
        configFilename = "appsettings.test.json";
        appsettingsResName = manifestResourceNames
            .FirstOrDefault(r => r.EndsWith(configFilename, StringComparison.OrdinalIgnoreCase));
        if (appsettingsResName is null) return;
        resourceStream = GetType().GetTypeInfo().Assembly.GetManifestResourceStream(appsettingsResName);
        if (resourceStream is null) return;
        builder.AddJsonStream(resourceStream);
    }
}
