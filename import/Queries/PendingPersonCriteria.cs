namespace Shift.PendingPerson.Quickstart;

/// <summary>
/// Criteria shared by collect, count, search, and download endpoints.
/// Matches the CollectPendingPeople / CountPendingPeople / SearchPendingPeople
/// schemas in the API spec.
/// </summary>
public sealed class PendingPersonCriteria
{
    public Guid? SubmittedBy { get; set; }
    public Guid? PersonId { get; set; }
    public Guid? UserId { get; set; }

    public string? PersonCode { get; set; }
    public string? UserEmail { get; set; }
    public string? UserFirstName { get; set; }
    public string? UserLastName { get; set; }

    public DateTimeOffset? SubmittedBefore { get; set; }
    public DateTimeOffset? SubmittedSince { get; set; }
}
