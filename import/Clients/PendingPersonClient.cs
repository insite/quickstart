using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Shift.PendingPerson.Quickstart;

/// <summary>
/// Lightweight client for the Shift Directory Pending-Person endpoints
/// (/api/directory/pending-people).
/// </summary>
public sealed class PendingPersonClient : IDisposable
{
    private const string BasePath = "api/directory/pending-people";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _http;

    public PendingPersonClient(HttpClient http)
    {
        _http = http;
    }

    // ------------------------------------------------------------------
    //  Single-record operations
    // ------------------------------------------------------------------

    /// <summary>
    /// HEAD /api/directory/pending-people/{pending} — true when the record exists.
    /// </summary>
    public async Task<bool> AssertAsync(Guid pendingId, CancellationToken ct = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Head, $"{BasePath}/{pendingId}");
        using var response = await _http.SendAsync(request, ct);
        return response.StatusCode == HttpStatusCode.OK;
    }

    /// <summary>GET /api/directory/pending-people/{pending} — returns null on 404.</summary>
    public async Task<PendingPersonModel?> RetrieveAsync(Guid pendingId, CancellationToken ct = default)
    {
        using var response = await _http.GetAsync($"{BasePath}/{pendingId}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PendingPersonModel>(JsonOptions, ct);
    }

    /// <summary>
    /// POST /api/directory/pending-people — returns the created record, or null on validation
    /// failure / duplicate.
    /// </summary>
    public async Task<PendingPersonModel?> CreateAsync(CreatePendingPerson command, CancellationToken ct = default)
    {
        using var response = await _http.PostAsJsonAsync(BasePath, command, JsonOptions, ct);

        if (response.StatusCode == HttpStatusCode.BadRequest)
            return null;

        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PendingPersonModel>(JsonOptions, ct);
    }

    /// <summary>PUT /api/directory/pending-people/{pending} — false on 404.</summary>
    public async Task<bool> ModifyAsync(Guid pendingId, ModifyPendingPerson command, CancellationToken ct = default)
    {
        using var response = await _http.PutAsJsonAsync($"{BasePath}/{pendingId}", command, JsonOptions, ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        response.EnsureSuccessStatusCode();
        return true;
    }

    /// <summary>DELETE /api/directory/pending-people/{pending} — false on 404.</summary>
    public async Task<bool> DeleteAsync(Guid pendingId, CancellationToken ct = default)
    {
        using var response = await _http.DeleteAsync($"{BasePath}/{pendingId}", ct);

        if (response.StatusCode == HttpStatusCode.NotFound)
            return false;

        response.EnsureSuccessStatusCode();
        return true;
    }

    // ------------------------------------------------------------------
    //  Bulk queries
    // ------------------------------------------------------------------

    /// <summary>
    /// POST /api/directory/pending-people/collect — page of records matching the criteria.
    /// </summary>
    public async Task<List<PendingPersonModel>> CollectAsync(
        PendingPersonCriteria criteria,
        QueryFilter? filter = null,
        CancellationToken ct = default)
    {
        using var response = await _http.PostAsJsonAsync(
            BuildUrl("collect", filter), criteria, JsonOptions, ct);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<PendingPersonModel>>(JsonOptions, ct)
            ?? [];
    }

    /// <summary>POST /api/directory/pending-people/count — number of matching records.</summary>
    public async Task<CountResult> CountAsync(
        PendingPersonCriteria criteria,
        QueryFilter? filter = null,
        CancellationToken ct = default)
    {
        using var response = await _http.PostAsJsonAsync(
            BuildUrl("count", filter), criteria, JsonOptions, ct);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CountResult>(JsonOptions, ct)
            ?? new CountResult();
    }

    /// <summary>POST /api/directory/pending-people/search — display-friendly matches.</summary>
    public async Task<List<PendingPersonMatch>> SearchAsync(
        PendingPersonCriteria criteria,
        QueryFilter? filter = null,
        CancellationToken ct = default)
    {
        using var response = await _http.PostAsJsonAsync(
            BuildUrl("search", filter), criteria, JsonOptions, ct);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<PendingPersonMatch>>(JsonOptions, ct)
            ?? [];
    }

    /// <summary>POST /api/directory/pending-people/download — raw export file bytes.</summary>
    public async Task<byte[]> DownloadAsync(
        PendingPersonCriteria criteria,
        QueryFilter? filter = null,
        CancellationToken ct = default)
    {
        using var response = await _http.PostAsJsonAsync(
            BuildUrl("download", filter), criteria, JsonOptions, ct);

        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync(ct);
    }

    // ------------------------------------------------------------------
    //  Helpers
    // ------------------------------------------------------------------

    private static string BuildUrl(string action, QueryFilter? filter)
    {
        var path = $"{BasePath}/{action}";

        if (filter is null)
            return path;

        var query = new StringBuilder();

        void Append(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;
            query.Append(query.Length == 0 ? '?' : '&');
            query.Append(Uri.EscapeDataString(key));
            query.Append('=');
            query.Append(Uri.EscapeDataString(value));
        }

        Append("page", filter.Page.ToString());
        Append("pageSize", filter.PageSize.ToString());
        Append("sort", filter.Sort);
        Append("excludes", filter.Excludes);
        Append("includes", filter.Includes);
        Append("format", filter.Format);

        return path + query;
    }

    public void Dispose() => _http.Dispose();
}
