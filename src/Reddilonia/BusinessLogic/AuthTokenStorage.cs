using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Reddit.Client.Dtos;

namespace Reddilonia.BusinessLogic;

public interface IAuthTokenStorage
{
    OAuthToken? Load();
    Task StoreToken(OAuthToken token);
}

public class AuthTokenStorage : IAuthTokenStorage
{
    private readonly ILogger<AuthTokenStorage> _logger;
    private const string Path = "secrets/settings.json";
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { WriteIndented = true };

    public AuthTokenStorage(ILogger<AuthTokenStorage> logger)
    {
        _logger = logger;
    }

    public OAuthToken? Load()
    {
        try
        {
            // TODO: use platform specific paths
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            if (!File.Exists(folderPath + "/" + Path)) return null;

            var data = File.ReadAllText(folderPath + "/" + Path);
            return JsonSerializer.Deserialize<OAuthToken>(data);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to read token from storage: {Message}", e.Message);
            return null;
        }
    }

    public async Task StoreToken(OAuthToken token)
    {
        try
        {
            // TODO: use platform specific paths
            var folderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            var serialized = JsonSerializer.Serialize(token, _jsonSerializerOptions);

            var dirPath = System.IO.Path.GetDirectoryName(Path);
            if (!string.IsNullOrWhiteSpace(dirPath))
                Directory.CreateDirectory(folderPath + "/" + dirPath);

            await File.WriteAllTextAsync(folderPath + "/" + Path, serialized).ConfigureAwait(false);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Unable to store new token: {Message}", e.Message);
            throw;
        }
    }
}
