using System.Globalization;

namespace LtiLaunch.Api.Lti;

public class LtiParameters
{
    private readonly Dictionary<string, string> _parameters = new(StringComparer.Ordinal);

    public string HttpMethod { get; }

    public LtiParameters(string httpMethod)
    {
        HttpMethod = httpMethod;

        _parameters["oauth_callback"] = "about:blank";
        _parameters["oauth_nonce"] = Guid.NewGuid().ToString("N");
        _parameters["oauth_signature_method"] = "HMAC-SHA256";
        _parameters["oauth_timestamp"] = ToUnixTimestamp(DateTime.UtcNow).ToString(CultureInfo.InvariantCulture);
        _parameters["oauth_version"] = "1.0";

        _parameters["launch_presentation_locale"] = CultureInfo.CurrentCulture.Name;
        _parameters["lti_message_type"] = "basic-lti-launch-request";
        _parameters["lti_version"] = "LTI-1p0";
    }

    public void Add(string name, string? value) => _parameters[name] = value ?? string.Empty;

    public void Add(string name, params LtiRole[] roles) =>
        _parameters[name] = roles.Length == 0 ? string.Empty : string.Join(",", roles);

    public bool Contains(string name) => _parameters.ContainsKey(name);

    public Dictionary<string, string> GetParameters() => new(_parameters, StringComparer.Ordinal);

    private static long ToUnixTimestamp(DateTime value) =>
        new DateTimeOffset(DateTime.SpecifyKind(value, DateTimeKind.Utc)).ToUnixTimeSeconds();
}
