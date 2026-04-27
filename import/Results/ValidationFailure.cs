namespace Shift.PendingPerson.Quickstart;

/// <summary>Validation failure returned by the API (400 Bad Request).</summary>
public sealed class ValidationFailure
{
    public List<Problem> Errors { get; set; } = [];
}
