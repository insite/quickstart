using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace BearerAuthDemo;

public class GradebookDemo
{
    private ApiClient _client;

    public async Task RunAsync(ApiClient client, Action<string> output, string criteria)
    {
        _client = client;

        var count = await CountGradebooksAsync();

        var gradebooks = await GetGradebooksAsync(criteria);

        var report = GradebookReport.Generate(count, criteria, gradebooks);

        output(report);
    }

    private async Task<int> CountGradebooksAsync()
    {
        const string url = "api/progress/gradebooks/count";

        var response = await _client.GetAsync(url);
        response.EnsureSuccess("GET", url);

        return JsonConvert.DeserializeObject<CountResult>(response.Data, JsonSettings.Default).Count;
    }

    private async Task<int> CountGradebooksAsync(string title)
    {
        const string url = "api/progress/gradebooks/count";

        var parameters = new Dictionary<string, string> { ["GradebookTitle"] = title };

        // Count using an HTTP GET request:
        var getResponse = await _client.GetAsync(url, parameters);
        getResponse.EnsureSuccess("GET", url);
        var a = JsonConvert.DeserializeObject<CountResult>(getResponse.Data, JsonSettings.Default).Count;

        // Count using an HTTP POST request:
        var postResponse = await _client.PostAsync(url, new { GradebookTitle = title });
        postResponse.EnsureSuccess("POST", url);
        var b = JsonConvert.DeserializeObject<CountResult>(postResponse.Data, JsonSettings.Default).Count;

        Debug.Assert(a == b);

        return b;
    }

    private async Task<Gradebook[]> GetGradebooksAsync(string title)
    {
        const string url = "api/progress/gradebooks";

        var parameters = string.IsNullOrEmpty(title)
            ? null
            : new Dictionary<string, string> { ["GradebookTitle"] = title };

        var response = await _client.GetAsync(url, parameters);
        response.EnsureSuccess("GET", url);

        return JsonConvert.DeserializeObject<Gradebook[]>(response.Data, JsonSettings.Default)
            .OrderBy(x => x.GradebookTitle)
            .ToArray();
    }
}
