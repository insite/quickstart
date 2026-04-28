namespace LtiLaunch.Api.Lti;

public class LtiDefaults
{
    public string LearnerCode { get; set; } = string.Empty;
    public string LearnerEmail { get; set; } = string.Empty;
    public string LearnerNameFirst { get; set; } = string.Empty;
    public string LearnerNameLast { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public string OrganizationIdentifier { get; set; } = string.Empty;
    public string OrganizationSecret { get; set; } = string.Empty;
    public string LaunchUrl { get; set; } = string.Empty;
}

public record TicketRequest(
    string LearnerCode,
    string LearnerEmail,
    string LearnerNameFirst,
    string LearnerNameLast,
    string GroupName,
    string OrganizationIdentifier,
    string OrganizationSecret,
    string LaunchUrl);

public record TicketResponse(string Url, IDictionary<string, string> Parameters);
