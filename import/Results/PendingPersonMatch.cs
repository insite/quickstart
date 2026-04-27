namespace Shift.PendingPerson.Quickstart;

/// <summary>Search result returned by the search endpoint.</summary>
public sealed class PendingPersonMatch
{
    public Guid PendingId { get; set; }
    public string? PendingStatus { get; set; }

    public DateTimeOffset SubmittedAt { get; set; }
    public string? SubmittedWhen { get; set; }
    public Guid SubmittedBy { get; set; }
    public string? SubmittedByName { get; set; }

    public Guid? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }

    public Guid? PersonId { get; set; }
    public string? PersonCode { get; set; }
}
