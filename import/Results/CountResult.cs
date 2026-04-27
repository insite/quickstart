namespace Shift.PendingPerson.Quickstart;

/// <summary>Wrapper returned by the count endpoint.</summary>
public sealed class CountResult
{
    public int Count { get; set; }
    public string? Summary { get; set; }
}
