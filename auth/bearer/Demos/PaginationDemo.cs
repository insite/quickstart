using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace BearerAuthDemo;

public class PaginationDemo
{
    private const int PageSize = 4;

    public async Task RunAsync(ApiClient client, Action<string> output)
    {
        output(await GetGradebooksInPagesAsync(client));
    }

    private static async Task<string> GetGradebooksInPagesAsync(ApiClient client)
    {
        var report = new StringBuilder();

        report.AppendLine("# PAGINATION DEMO");

        var page = 1;

        var (pagination, items) = await GetGradebooksPageAsync(client, page, PageSize);

        var totalPageCount = pagination.CountPages();

        report.AppendLine($"Watch me page through {pagination.TotalCount} gradebooks using a page size of {PageSize} items per page!");

        while (items.Length != 0)
        {
            report.AppendLine($"  - Page {pagination.Page} of {totalPageCount} has {items.Length} items ({(pagination.HasMore() ? "more" : "no more")})");

            (pagination, items) = await GetGradebooksPageAsync(client, ++page, PageSize);
        }

        return report.ToString();
    }

    private static async Task<(Pagination, Gradebook[])> GetGradebooksPageAsync(ApiClient client, int page, int take)
    {
        const string url = "api/progress/gradebooks";

        var parameters = new Dictionary<string, string>
        {
            ["filter.page"] = page.ToString(),
            ["filter.pagesize"] = take.ToString(),
        };

        var response = await client.GetAsync(url, parameters);
        response.EnsureSuccess("GET", url);

        var items = JsonConvert.DeserializeObject<Gradebook[]>(response.Data, JsonSettings.Default);
        var pagination = JsonConvert.DeserializeObject<Pagination>(response.Headers[Pagination.HeaderKey], JsonSettings.Default);

        return (pagination, items);
    }
}
