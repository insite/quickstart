using System;
using System.Linq;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace BearerAuthDemo;

public class UserDemo
{
    private ApiClient _client;

    public async Task RunAsync(ApiClient client, Action<string> output)
    {
        _client = client;

        var users = await GetUsersAsync();

        var report = UserReport.Generate(users);

        output(report);
    }

    private async Task<User[]> GetUsersAsync()
    {
        const string url = "api/security/users";

        var response = await _client.GetAsync(url);
        response.EnsureSuccess("GET", url);

        return [.. JsonConvert.DeserializeObject<User[]>(response.Data, JsonSettings.Default)
            .OrderBy(x => x.FullName)];
    }
}
