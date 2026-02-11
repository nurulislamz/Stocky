namespace stockyapi.Controllers.Helpers;

public static class UserAgents
{
    // Chrome
    public static string ChromeUserAgent1 =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36";

    public static string ChromeUserAgent2 =
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36";

    public static string ChromeUserAgent3 =
        "Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36";

    // Firefox
    public static string FirefoxUserAgent1 =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:135.0) Gecko/20100101 Firefox/135.0";

    public static string FirefoxUserAgent2 =
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 14.7; rv:135.0) Gecko/20100101 Firefox/135.0";

    public static string FirefoxUserAgent3 = "Mozilla/5.0 (X11; Linux i686; rv:135.0) Gecko/20100101 Firefox/135.0";

    // Safari
    public static string SafariUserAgent1 =
        "Mozilla/5.0 (Macintosh; Intel Mac OS X 14_7_4) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.3 Safari/605.1.15";

    // Edge
    public static string EdgeUserAgent1 =
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/133.0.0.0 Safari/537.36 Edg/131.0.2903.86";

    public static IReadOnlyList<string> AllUserAgents =
    [
        ChromeUserAgent1, ChromeUserAgent2, ChromeUserAgent3, FirefoxUserAgent1, FirefoxUserAgent2, FirefoxUserAgent3,
        SafariUserAgent1, EdgeUserAgent1
    ];

    public static string GetRandomNewUserAgent(string? oldUserAgent = null)
    {
        if (oldUserAgent is not null)
        {
            var remainingUserAgents = AllUserAgents.Where(x => x != oldUserAgent).ToList();
            return remainingUserAgents[Random.Shared.Next(AllUserAgents.Count)];
        }
        return AllUserAgents[Random.Shared.Next(AllUserAgents.Count)];
    }

}