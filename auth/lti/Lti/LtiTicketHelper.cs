using System.Security.Cryptography;
using System.Text;

namespace LtiLaunch.Api.Lti;

public static class LtiTicketHelper
{
    private static readonly string[] UriRfc3986CharsToEscape = ["!", "*", "'", "(", ")"];

    private static readonly HashSet<string> ExcludedSignatureNames =
        new(StringComparer.Ordinal) { "debug", "oauth_signature", "realm" };

    public static LtiTicket GetTicket(string key, Uri url, LtiParameters parameters)
    {
        var p = parameters.GetParameters();
        foreach (var k in p.Where(pair => string.IsNullOrWhiteSpace(pair.Value)).Select(pair => pair.Key).ToList())
            p.Remove(k);

        var signature = GenerateSignature(key, parameters.HttpMethod, url, p);
        return new LtiTicket(url, signature, p);
    }

    public static string GenerateSignature(string key, string httpMethod, Uri url, Dictionary<string, string> parameters)
    {
        var combined = new Dictionary<string, string>(parameters, StringComparer.Ordinal);

        foreach (var (queryKey, queryValue) in ParseQueryString(url.Query))
            combined[queryKey] = queryValue;

        var signatureBase = GenerateSignatureBase(httpMethod, url, combined);
        var data = Encoding.ASCII.GetBytes(signatureBase);
        var keyBytes = Encoding.ASCII.GetBytes($"{EscapeUriDataStringRfc3986(key)}&");

        var hash = HMACSHA256.HashData(keyBytes, data);
        return Convert.ToBase64String(hash);
    }

    private static string GenerateSignatureBase(string httpMethod, Uri url, Dictionary<string, string> parameters)
    {
        var result = new StringBuilder();

        result.Append(EscapeUriDataStringRfc3986(httpMethod).ToUpperInvariant()).Append('&');

        var normalizedUrl = $"{url.Scheme.ToLowerInvariant()}://{url.Host.ToLowerInvariant()}";
        if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            normalizedUrl += ":" + url.Port;
        normalizedUrl += url.AbsolutePath;

        result.Append(EscapeUriDataStringRfc3986(normalizedUrl)).Append('&');
        result.Append(EscapeUriDataStringRfc3986(ToNormalizedString(parameters)));

        return result.ToString();
    }

    private static string ToNormalizedString(Dictionary<string, string> parameters)
    {
        var list = new List<KeyValuePair<string, string>>();

        foreach (var (name, value) in parameters)
        {
            if (ExcludedSignatureNames.Contains(name))
                continue;

            list.Add(new KeyValuePair<string, string>(
                EscapeUriDataStringRfc3986(name),
                EscapeUriDataStringRfc3986(value ?? string.Empty)
            ));
        }

        list.Sort((left, right) =>
            left.Key.Equals(right.Key, StringComparison.Ordinal)
                ? string.Compare(left.Value, right.Value, StringComparison.Ordinal)
                : string.Compare(left.Key, right.Key, StringComparison.Ordinal)
        );

        var normalized = new StringBuilder();
        foreach (var pair in list)
            normalized.Append('&').Append(pair.Key).Append('=').Append(pair.Value);

        return normalized.ToString().TrimStart('&');
    }

    private static string EscapeUriDataStringRfc3986(string value)
    {
        var escaped = new StringBuilder(Uri.EscapeDataString(value));
        foreach (var s in UriRfc3986CharsToEscape)
            escaped.Replace(s, Uri.HexEscape(s[0]));

        return escaped.ToString();
    }

    private static IEnumerable<KeyValuePair<string, string>> ParseQueryString(string query)
    {
        if (string.IsNullOrEmpty(query)) yield break;

        var trimmed = query.StartsWith('?') ? query[1..] : query;
        foreach (var pair in trimmed.Split('&', StringSplitOptions.RemoveEmptyEntries))
        {
            var eq = pair.IndexOf('=');
            var key = eq < 0 ? pair : pair[..eq];
            var value = eq < 0 ? string.Empty : pair[(eq + 1)..];
            yield return new KeyValuePair<string, string>(Uri.UnescapeDataString(key), Uri.UnescapeDataString(value));
        }
    }
}
