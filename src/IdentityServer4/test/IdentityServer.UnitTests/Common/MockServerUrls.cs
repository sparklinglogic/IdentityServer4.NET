using IdentityServer4.Extensions;

namespace IdentityServer.UnitTests.Common;

public class MockServerUrls
{
    public string Origin { get; set; }
    public string BasePath { get; set; }

    public string BaseUrl => Origin + BasePath;
}

internal static class MockServerUrlExtensions
{
    public static string GetIdentityServerRelativeUrl(this MockServerUrls urls, string path)
    {
        if (!path.IsLocalUrl())
        {
            return null;
        }

        if (path.StartsWith("~/")) path = path.Substring(1);
        path = urls.BaseUrl + path.EnsureLeadingSlash();
        return path;
    }
}