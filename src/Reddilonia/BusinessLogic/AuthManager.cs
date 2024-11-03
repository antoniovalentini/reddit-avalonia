using System;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit.Client;

namespace Reddilonia.BusinessLogic;

public interface IAuthManager
{
    event EventHandler<AuthSuccessEventArgs>? AuthSuccess;
    void Start();
    void OpenBrowser();
    void Stop();
    void Dispose();
}

public class AuthManager : IDisposable, IAuthManager
{
    private readonly IRedditAuthClient _redditAuthClient;
    private readonly WebAuthParameters _webAuthParameters;
    private readonly HttpListener _listener;
    private readonly IAuthTokenStorage _authTokenStorage;
    private readonly ILogger<AuthManager> _logger;

    private const string Scope = "creddits%20modcontributors%20modmail%20modconfig%20subscribe%20structuredstyles%20vote%20wikiedit%20mysubreddits%20submit%20modlog%20modposts%20modflair%20save%20modothers%20read%20privatemessages%20report%20identity%20livemanage%20account%20modtraffic%20wikiread%20edit%20modwiki%20modself%20history%20flair";
    public event EventHandler<AuthSuccessEventArgs>? AuthSuccess;
    private CancellationTokenSource? _cancellationTokenSource;

    public AuthManager(IRedditAuthClient redditAuthClient, IOptions<RedditClientSettings> redditClientSettings, IAuthTokenStorage authTokenStorage, ILogger<AuthManager> logger)
    {
        _redditAuthClient = redditAuthClient;
        _webAuthParameters = redditClientSettings.Value.WebAuthParameters;
        _authTokenStorage = authTokenStorage;
        _logger = logger;

        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://{_webAuthParameters.Host}:" + _webAuthParameters.Port + "/");
    }

    public void Start()
    {
        _listener.Start();
        _ = DoStuff();
    }

    private async Task DoStuff()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        var contextTask = _listener.GetContextAsync();
        var result = await Task.WhenAny(contextTask, new CancellationTokenTaskSource<HttpListenerContext>(_cancellationTokenSource.Token).Task);

        if (result.IsCanceled)
        {
            _logger.LogWarning("Stopping application...");
            _listener.Stop();
            return;
        }

        var context = await result;

        const string responseString = "<html><body>Please return to the app.</body></html>";
        var buffer = Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentLength64 = buffer.Length;

        context.Response.StatusCode = (int) HttpStatusCode.OK;
        context.Response.ContentType = "text/html";
        await context.Response.OutputStream.WriteAsync(buffer.AsMemory(0, buffer.Length), _cancellationTokenSource.Token);
        context.Response.OutputStream.Close();
        _listener.Stop();

        string? code = null;
        // TODO: what do we do with the returned state?
        string? state = null;
        try
        {
            code = context.Request.QueryString["code"];
            state = context.Request.QueryString["state"];  // This app formats state as:  AppId + ":" [+ AppSecret]
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR: Request received without code and/or state! {Message}", ex.Message);
        }

        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(state))
        {
            _logger.LogError("ERROR:  code or state null");
            return;
        }

        try
        {
            var oAuthToken = await _redditAuthClient.ExchangeCode(code);
            await _authTokenStorage.StoreToken(oAuthToken);
            AuthSuccess?.Invoke(this, new AuthSuccessEventArgs(oAuthToken.AccessToken, oAuthToken.RefreshToken));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unexpected error while exchanging code");
        }
    }

    public void OpenBrowser()
    {
        var authUrl = "https://www.reddit.com/api/v1/authorize?client_id=" + _webAuthParameters.AppId
                      + "&response_type=code"
                      + "&state=" + _webAuthParameters.AppId + ":" + _webAuthParameters.AppSecret
                      + "&redirect_uri=http://" + _webAuthParameters.Host + ":" + _webAuthParameters.Port + "/" + _webAuthParameters.RelativeRedirectUri
                      + "&duration=permanent"
                      + "&scope=" + Scope;

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            try
            {
                Process.Start(new ProcessStartInfo { FileName = authUrl, UseShellExecute = true });
            }
            catch (System.ComponentModel.Win32Exception)
            {
                Process.Start(authUrl);
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // For OSX run a separate command to open the web browser as found in https://brockallen.com/2016/09/24/process-start-for-urls-on-net-core/
            Process.Start("open", authUrl);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Similar to OSX, Linux can (and usually does) use xdg for this task.
            Process.Start("xdg-open", authUrl);
        } else if (OperatingSystem.IsAndroid())
        {

        }
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
        if (_listener.IsListening) _listener.Stop();
    }

    public void Dispose()
    {
        Stop();
        _listener.Close();
        _logger.LogInformation("AuthManager disposed.");
    }
}

public record AuthSuccessEventArgs(string AccessToken, string RefreshToken);
