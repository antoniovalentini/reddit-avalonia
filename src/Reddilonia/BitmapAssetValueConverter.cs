using System;
using System.Globalization;
using System.Web;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using Microsoft.Extensions.DependencyInjection;

namespace Reddilonia;

public class BitmapAssetValueConverter : IValueConverter
{
    private readonly MemoryStreamWebRetriever? _memoryStreamWeb;

    public static BitmapAssetValueConverter Instance { get; } = new();
    public BitmapAssetValueConverter()
    {
        _memoryStreamWeb = App.Instance.ServiceProvider?.GetService<MemoryStreamWebRetriever>();
    }

    public BitmapAssetValueConverter(MemoryStreamWebRetriever memoryStreamWeb)
    {
        _memoryStreamWeb = memoryStreamWeb;
    }

    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value == null) return null;

        if (value is not string rawUri || !targetType.IsAssignableFrom(typeof(Bitmap)))
        {
            throw new NotSupportedException();
        }

        var decoded = HttpUtility.HtmlDecode(rawUri);
        if (!Uri.TryCreate(decoded, UriKind.Absolute, out var uri))
        {
            return null;
        }

        if (_memoryStreamWeb is null) return null;
        var stream = _memoryStreamWeb.GetMemoryStreamAsync(uri).ConfigureAwait(false).GetAwaiter().GetResult();
        return stream is null ? null : new Bitmap(stream);
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
