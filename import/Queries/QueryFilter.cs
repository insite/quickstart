namespace Shift.PendingPerson.Quickstart;

/// <summary>
/// Paging, sorting, and projection options. Sent as query-string parameters
/// alongside the request body on collect / count / search / download calls.
/// </summary>
public sealed class QueryFilter
{
    /// <summary>One-based page index. Zero disables paging.</summary>
    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;

    /// <summary>
    /// Comma-separated property names, with optional "+asc" / "+desc" suffixes.
    /// Example: "UserLastName+asc,SubmittedAt+desc".
    /// </summary>
    public string? Sort { get; set; }

    public string? Excludes { get; set; }
    public string? Includes { get; set; }

    /// <summary>Export file format used by the download endpoint (e.g. "json", "csv").</summary>
    public string? Format { get; set; }
}
