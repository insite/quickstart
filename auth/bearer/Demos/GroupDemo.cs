using System;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace BearerAuthDemo;

public class GroupDemo
{
    private ApiClient _client;

    public async Task RunAsync(ApiClient client, Action<string> output)
    {
        _client = client;

        var groups = await GetGroupsAsync();

        var report = GroupReport.Generate(groups);

        output(report);
    }

    private async Task<Group[]> GetGroupsAsync()
    {
        const string url = "api/directory/groups";

        var response = await _client.GetAsync(url);
        response.EnsureSuccess("GET", url);

        return JsonConvert.DeserializeObject<Group[]>(response.Data, JsonSettings.Default);
    }
}
