using System.Net.Http.Json;

using Microsoft.Extensions.Configuration;

using Shift.PendingPerson.Quickstart;

// -------------------------------------------------------------------------------------------------
//  Configuration — values come from appsettings.json, with appsettings.work.json
//  layered on top for local development/work environment overrides (gitignored, never committed).
// -------------------------------------------------------------------------------------------------

var configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
    .AddJsonFile("appsettings.work.json", optional: true, reloadOnChange: false)
    .Build();

var baseUrl = configuration["BaseUrl"]
    ?? throw new InvalidOperationException("BaseUrl is not configured. Set it in appsettings.json or appsettings.work.json.");

var clientSecret = configuration["ClientSecret"]
    ?? throw new InvalidOperationException("ClientSecret is not configured. Set it in appsettings.work.json (preferred) or appsettings.json.");

// -------------------------------------------------------------------------------------------------
//  HTTP client setup — exchange the client secret for a short-lived JWT
// -------------------------------------------------------------------------------------------------

using var http = new HttpClient { BaseAddress = new Uri(baseUrl) };

var bearerToken = await AcquireAccessTokenAsync(http, clientSecret);

http.DefaultRequestHeaders.Add("Authorization", $"Bearer {bearerToken}");

var client = new PendingPersonClient(http);

static async Task<string> AcquireAccessTokenAsync(HttpClient http, string secret)
{
    var fourWeeks = 2419200; // seconds

    using var response = await http.PostAsJsonAsync(
        "api/security/tokens/generate",
        new { Secret = secret, Lifetime = fourWeeks });

    response.EnsureSuccessStatusCode();

    var jwt = await response.Content.ReadFromJsonAsync<Jwt>()
        ?? throw new InvalidOperationException("Token endpoint returned an empty response.");

    Console.WriteLine($"Acquired {jwt.TokenType} access token (expires in {jwt.ExpiresIn / 60:n0} minutes) - token lifetime is {jwt.Lifetime}\n");

    return jwt.AccessToken;
}

var pendingId = Guid.NewGuid();

// -------------------------------------------------------------------------------------------------
//  1. CREATE — POST api/directory/pending-people
// -------------------------------------------------------------------------------------------------

Console.WriteLine("1. CREATE  POST api/directory/pending-people");

var createCommand = new CreatePendingPerson
{
    PendingId = pendingId,
    PersonCode = "EMP-4710",
    UserEmail = "jane.doe@example.com",
    UserFirstName = "Jane",
    UserLastName = "Doe"
};

var created = await client.CreateAsync(createCommand);

if (created is null)
{
    Console.WriteLine($"   Create failed (duplicate or validation failure) for {pendingId}.");
    return;
}

Console.WriteLine($"   Created pending-person {created.PendingId} at {created.SubmittedAt:O}");

// -------------------------------------------------------------------------------------------------
//  2. ASSERT — HEAD api/directory/pending-people/{pending}
// -------------------------------------------------------------------------------------------------

Console.WriteLine("\n2. ASSERT  HEAD api/directory/pending-people/{pending}");

var exists = await client.AssertAsync(pendingId);
Console.WriteLine($"   Exists: {exists}");

// -------------------------------------------------------------------------------------------------
//  3. RETRIEVE — GET api/directory/pending-people/{pending}
// -------------------------------------------------------------------------------------------------

Console.WriteLine("\n3. RETRIEVE  GET api/directory/pending-people/{pending}");

var person = await client.RetrieveAsync(pendingId);

if (person is null)
{
    Console.WriteLine("   Not found.");
    return;
}

Console.WriteLine($"   PendingId:   {person.PendingId}");
Console.WriteLine($"   Name:        {person.UserFirstName} {person.UserLastName}");
Console.WriteLine($"   Email:       {person.UserEmail}");
Console.WriteLine($"   PersonCode:  {person.PersonCode}");
Console.WriteLine($"   SubmittedAt: {person.SubmittedAt:O}");

// -------------------------------------------------------------------------------------------------
//  4. MODIFY — PUT api/directory/pending-people/{pending}
// -------------------------------------------------------------------------------------------------

Console.WriteLine("\n4. MODIFY  PUT api/directory/pending-people/{pending}");

var modifyCommand = new ModifyPendingPerson
{
    PendingId = pendingId,
    PersonCode = person.PersonCode,
    UserEmail = person.UserEmail,
    UserFirstName = person.UserFirstName,
    UserLastName = "Smith"                            // name change
};

var modified = await client.ModifyAsync(pendingId, modifyCommand);
Console.WriteLine(modified
    ? "   Modified successfully."
    : "   Not found — nothing to modify.");

// -------------------------------------------------------------------------------------------------
//  5. COUNT — POST api/directory/pending-people/count
// -------------------------------------------------------------------------------------------------

Console.WriteLine("\n5. COUNT  POST api/directory/pending-people/count");

var countCriteria = new PendingPersonCriteria
{
    UserLastName = "Smith"
};

var count = await client.CountAsync(countCriteria);
Console.WriteLine($"   Matching records: {count.Count}");
if (!string.IsNullOrEmpty(count.Summary))
    Console.WriteLine($"   Summary: {count.Summary}");

// -------------------------------------------------------------------------------------------------
//  6. COLLECT — POST api/directory/pending-people/collect
// -------------------------------------------------------------------------------------------------

Console.WriteLine("\n6. COLLECT  POST api/directory/pending-people/collect  (page 1, size 5)");

var collectFilter = new QueryFilter
{
    Page = 1,
    PageSize = 5,
    Sort = "UserLastName,UserFirstName"
};

var page = await client.CollectAsync(new PendingPersonCriteria(), collectFilter);

foreach (var item in page)
    Console.WriteLine($"   {item.PendingId}  {item.UserFirstName} {item.UserLastName}  ({item.UserEmail})");

if (page.Count == 0)
    Console.WriteLine("   (no results)");

// -------------------------------------------------------------------------------------------------
//  7. SEARCH — POST api/directory/pending-people/search
// -------------------------------------------------------------------------------------------------

Console.WriteLine("\n7. SEARCH  POST api/directory/pending-people/search");

var searchCriteria = new PendingPersonCriteria { UserLastName = "Smith" };
var searchFilter = new QueryFilter { Page = 1, PageSize = 10 };

var matches = await client.SearchAsync(searchCriteria, searchFilter);

foreach (var m in matches)
    Console.WriteLine($"   {m.PendingId}  {m.UserFirstName} {m.UserLastName}  submitted {m.SubmittedWhen} by {m.SubmittedByName}");

if (matches.Count == 0)
    Console.WriteLine("   (no results)");

// -------------------------------------------------------------------------------------------------
//  8. DOWNLOAD — POST api/directory/pending-people/download
// -------------------------------------------------------------------------------------------------

Console.WriteLine("\n8. DOWNLOAD  POST api/directory/pending-people/download");

var downloadFilter = new QueryFilter { Format = "json" };

var bytes = await client.DownloadAsync(new PendingPersonCriteria(), downloadFilter);
var downloadPath = Path.Combine(Environment.CurrentDirectory, "sample-export.json");
await File.WriteAllBytesAsync(downloadPath, bytes);
Console.WriteLine($"   Saved {bytes.Length:N0} bytes to {downloadPath}");

// -------------------------------------------------------------------------------------------------
//  9. DELETE — DELETE api/directory/pending-people/{pending}
// -------------------------------------------------------------------------------------------------

Console.WriteLine("\n9. DELETE  DELETE api/directory/pending-people/{pending}");

var deleted = await client.DeleteAsync(pendingId);
Console.WriteLine(deleted
    ? $"   Deleted {pendingId}"
    : "   Not found — already deleted?");

// -------------------------------------------------------------------------------------------------

Console.WriteLine("\nDone. All api/directory/pending-people endpoints exercised.");

file sealed record Jwt(string AccessToken, string TokenType, int ExpiresIn, string Lifetime);
