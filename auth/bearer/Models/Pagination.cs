namespace BearerAuthDemo;

/// <summary>
/// Supports page-based pagination
/// </summary>
public class Pagination
{
    public const string HeaderKey = "X-Query-Pagination";

    /// <summary>
    /// The page number to retrieve (page numbers start at 1)
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// The number of items requested per page
    /// </summary>
    public int PageSize { get; set; }

    /// <summary>
    /// The total number of items in the data set
    /// </summary>
    public int TotalCount { get; set; }

    /// <summary>
    /// The total number of pages in the data set
    /// </summary>
    public int CountPages() => (int)System.Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// True if this is not the last page
    /// </summary>
    public bool HasMore() => Page < CountPages();
}