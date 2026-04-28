namespace LtiLaunch.Api.Lti;

public class LtiTicket(Uri url, string signature, Dictionary<string, string> parameters)
{
    public Uri Url { get; } = url;

    public string Signature { get; } = signature;

    public Dictionary<string, string> Parameters { get; } = parameters;
}
