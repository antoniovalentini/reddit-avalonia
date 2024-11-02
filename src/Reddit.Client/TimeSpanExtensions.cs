namespace Reddit.Client;

public static class TimeSpanExtensions
{
    /// <summary>
    /// Converts a Unix timestamp to a human-readable relative time string (e.g., "2h", "3d", "1y").
    /// </summary>
    /// <param name="createdUtc">Unix timestamp in seconds since epoch. If null, defaults to 0.</param>
    /// <returns>
    /// A formatted string representing the time elapsed since the timestamp, with the following units:
    /// - "s" for seconds (less than 1 minute)
    /// - "m" for minutes (less than 1 hour)
    /// - "h" for hours (less than 1 day)
    /// - "d" for days (less than 1 week)
    /// - "w" for weeks (less than 28 days)
    /// - "m" for months (less than 1 year)
    /// - "y" for years
    /// </returns>
    /// <example>
    /// <code>
    /// double? timestamp = 1678901234; // March 15, 2023
    /// string elapsed = timestamp.TimeSpanFromCreationEpoch(); // Returns something like "1y"
    /// </code>
    /// </example>
    /// <remarks>
    /// The method rounds the time difference to the nearest whole unit.
    /// For null input, the method treats it as Unix epoch (January 1, 1970).
    /// </remarks>
    public static string TimeSpanFromCreationEpoch(this double? createdUtc)
    {
        var diff = DateTime.UtcNow - DateTimeOffset.FromUnixTimeSeconds((long)(createdUtc ?? 0)).DateTime;
        if (diff.TotalSeconds < 60) return (int)Math.Round(diff.TotalSeconds) + "s";
        if (diff.TotalMinutes < 60) return (int)Math.Round(diff.TotalMinutes) + "m";
        if (diff.TotalHours < 24) return (int)Math.Round(diff.TotalHours) + "h";

        return diff.TotalDays switch
        {
            < 7 => (int)Math.Round(diff.TotalDays) + "d",
            < 28 => (int)Math.Round(diff.TotalDays / 7) + "w",
            < 365 => (int)Math.Round(diff.TotalDays / 28) + "m",
            _ => (int)Math.Round(diff.TotalDays / 365) + "y"
        };
    }
}
