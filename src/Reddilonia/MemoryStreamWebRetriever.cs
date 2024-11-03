using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Reddilonia;

public class MemoryStreamWebRetriever
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<MemoryStreamWebRetriever> _logger;

    private static readonly Dictionary<string, byte[]> Cache = new();

    public MemoryStreamWebRetriever(HttpClient httpClient, ILogger<MemoryStreamWebRetriever> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<MemoryStream?> GetMemoryStreamAsync(Uri uri)
    {
        try
        {
            if (Cache.TryGetValue(uri.ToString(), out var bytes))
            {
                return new MemoryStream(bytes);
            }
            var data = await _httpClient.GetByteArrayAsync(uri).ConfigureAwait(false);
            Cache[uri.ToString()] = data;
            return new MemoryStream(data);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "An error occurred while downloading image '{uri}' : {Message}", uri, ex.Message);
            return null;
        }
    }
}
