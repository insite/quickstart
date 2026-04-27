namespace Shift.PendingPerson.Quickstart;

/// <summary>Single property-level error entry inside a <see cref="ValidationFailure"/>.</summary>
public sealed class Problem
{
    public string? PropertyName { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorCode { get; set; }
}
