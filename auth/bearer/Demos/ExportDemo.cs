using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace BearerAuthDemo;

public class ExportDemo
{
    private ApiClient _client;

    public async Task StartExportAsync(ApiClient client, Action<string> output, string criteria)
    {
        _client = client;

        var export = await ExportGradebooksAsync(criteria);

        var downloadUrl = $"{_client.BaseUrl}/{export.DownloadUrl}";

        var lifetime = (int)(new DateTimeOffset(export.Expiry) - DateTimeOffset.UtcNow).TotalMinutes;

        output($"Here is the link to download your gradebooks: {downloadUrl}");

        output($"Please note the link requires authentication, and it expires in {lifetime} minutes.");
    }

    private async Task<Export> ExportGradebooksAsync(string title)
    {
        const string url = "api/progress/gradebooks/export";

        var parameters = string.IsNullOrEmpty(title)
            ? null
            : new Dictionary<string, string> { ["GradebookTitle"] = title };

        var response = await _client.GetAsync(url, parameters);
        response.EnsureSuccess("GET", url);

        return JsonConvert.DeserializeObject<Export>(response.Data, JsonSettings.Default);
    }
}
