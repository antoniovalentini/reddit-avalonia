using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Reddit.Client.Dtos;

namespace Reddit.Client;

public class RedditAuthClient : IRedditAuthClient
{
    private readonly RedditClientSettings _settings;
    private readonly HttpClient _httpClient;
    private readonly ILogger<IRedditAuthClient> _logger;
    private const string DefaultUserAgent = "RedditClient/1.0";

    public RedditAuthClient(IOptions<RedditClientSettings> settings, HttpClient httpClient, ILogger<IRedditAuthClient> logger)
    {
        _settings = settings.Value;
        _httpClient = httpClient;
        _logger = logger;
        if (_httpClient.DefaultRequestHeaders.UserAgent.Count == 0)
        {
            _httpClient.DefaultRequestHeaders.Add("User-Agent", DefaultUserAgent);
        }
    }

    public OAuthToken? CurrentOAuthToken { get; private set; }

    public async Task<OAuthToken> ExchangeCode(string code)
    {
        var requestContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new("grant_type", "authorization_code"),
            new("code", code),
            new("redirect_uri", "http://" + _settings.WebAuthParameters.Host + ":" + _settings.WebAuthParameters.Port + "/" + _settings.WebAuthParameters.RelativeRedirectUri), // This must be an EXACT match in the app settings on Reddit
        });

        return await Post<OAuthToken>("api/v1/access_token", requestContent);
    }

    public async Task<OAuthToken> GetScriptAppToken()
    {
        var requestContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new("grant_type", "password"),
            new("username", _settings.WebAuthParameters.Username),
            new("password", _settings.WebAuthParameters.Password),
        });

        var oAuthToken = await Post<OAuthToken>("api/v1/access_token", requestContent);
        CurrentOAuthToken = oAuthToken;
        return oAuthToken;
    }

    public async Task<OAuthToken> RefreshToken(string refreshToken)
    {
        var requestContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
        {
            new("grant_type", "refresh_token"),
            new("refresh_token", refreshToken),
        });

        var oAuthToken = await Post<OAuthToken>("api/v1/access_token", requestContent);
        CurrentOAuthToken = oAuthToken;

        return oAuthToken;
    }

    private async Task<T> Post<T>(string url, HttpContent content)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, url) { Content = content };
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes(_settings.WebAuthParameters.AppId + ":" + _settings.WebAuthParameters.AppSecret)));
        var response = await _httpClient.SendAsync(request);
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode || responseContent.Contains("invalid_grant"))
        {
            _logger.LogError("Failed to send request to {Url}: {ResponseContent}", url, responseContent);
            throw new Exception($"Failed to send request to {url}: {responseContent}");
        };
        var result = JsonSerializer.Deserialize<T>(responseContent);
        if (result is null) throw new Exception("Failed to deserialize response: " + responseContent);
        return result;
    }
}
