using System.Text.Json;

namespace stockytests.Mocks;

/// <summary>
/// Loads mock call JSON files from the YahooMockCalls directory.
/// </summary>
public static class MockCallLoader
{
    private static readonly string MockCallsPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "..", "..", "..", "Integration", "YahooMockCalls");

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Loads a single mock call file by name.
    /// </summary>
    /// <param name="fileName">The JSON file name (e.g., "chart-AAPL-...-1m.static.json")</param>
    /// <returns>The deserialized mock call data</returns>
    public static MockCallFile LoadMockCall(string fileName)
    {
        var fullPath = Path.Combine(MockCallsPath, fileName);
        var json = File.ReadAllText(fullPath);
        return JsonSerializer.Deserialize<MockCallFile>(json, JsonOptions)
               ?? throw new InvalidOperationException($"Failed to deserialize mock call file: {fileName}");
    }

    /// <summary>
    /// Gets all mock call file names that match a given prefix.
    /// </summary>
    /// <param name="prefix">The file name prefix (e.g., "chart-", "historical-")</param>
    /// <returns>Collection of matching file names</returns>
    public static IEnumerable<string> GetMockCallFilesByPrefix(string prefix)
    {
        var directory = new DirectoryInfo(MockCallsPath);
        if (!directory.Exists)
        {
            throw new DirectoryNotFoundException($"Mock calls directory not found: {MockCallsPath}");
        }

        return directory
            .GetFiles($"{prefix}*.json")
            .Select(f => f.Name);
    }

    /// <summary>
    /// Loads all mock calls that match a given prefix.
    /// </summary>
    /// <param name="prefix">The file name prefix</param>
    /// <returns>Collection of mock call data with their file names</returns>
    public static IEnumerable<(string FileName, MockCallFile MockCall)> LoadMockCallsByPrefix(string prefix)
    {
        foreach (var fileName in GetMockCallFilesByPrefix(prefix))
        {
            yield return (fileName, LoadMockCall(fileName));
        }
    }

    /// <summary>
    /// Checks if a mock call file exists.
    /// </summary>
    public static bool MockCallExists(string fileName)
    {
        return File.Exists(Path.Combine(MockCallsPath, fileName));
    }
}
