# Bearer Authentication Demo

A C# .NET 10 console application that demonstrates how to call the Shift iQ API using a JWT bearer token.

The example shows how to authenticate, make API calls, and process responses for groups, user accounts, and gradebooks. The API offers much more than this — the source code here is intended only as a starting point.

Full developer documentation: [docs.shiftiq.com/developers](https://docs.shiftiq.com/developers)


## Table of Contents

- [Overview](#overview)
- [Prerequisites](#prerequisites)
- [Quick Start](#quick-start)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [Usage](#usage)
- [Examples](#examples)
- [Building and Running](#building-and-running)


## Overview

This quickstart demonstrates:

- **Bearer token authentication** — exchange a client secret for a short-lived JWT, then send it on every request
- **Async-first HTTP client** built on `HttpClient`
- **JSON serialization/deserialization** with `Newtonsoft.Json` (case-insensitive by default)
- **Configuration management** with `appsettings.json` and environment variables
- **Pagination** via the `X-Query-Pagination` response header
- **Simple reporting** from API responses

The application calls these endpoints:

- `api/diagnostic/health` — server health probe
- `api/diagnostic/version` — server version
- `api/security/tokens/generate` — exchange a client secret for a JWT
- `api/directory/groups` — contact groups
- `api/security/users` — user accounts
- `api/progress/gradebooks` and `api/progress/gradebooks/count` — gradebooks


## Prerequisites

- **.NET 10.0 SDK** or later
- **Visual Studio 2022** or **VS Code** with the C# extension
- **API credentials** — a client secret and the base URL of your Shift iQ instance


## Quick Start

1. **Clone the repository**

   ```bash
   git clone https://github.com/insite/quickstart.git
   cd quickstart/auth/bearer
   ```

2. **Add your credentials**

   Create a local override file `appsettings.work.json` next to `appsettings.json`. This file is git-ignored, so your secret stays out of source control.

   ```json
   {
     "BaseUrl": "https://your-shiftiq-instance.example.com/",
     "ClientSecret": "your-client-secret"
   }
   ```

3. **Build and run**

   ```bash
   dotnet run
   ```


## Configuration

Settings are loaded in this order (later sources override earlier ones):

1. `appsettings.json` (committed; placeholders only)
2. `appsettings.work.json` (git-ignored; your real values)
3. Environment variables

### Properties

| Property         | Description                                  | Required | Default       |
|------------------|----------------------------------------------|----------|---------------|
| `BaseUrl`        | Base URL of your Shift iQ instance, with trailing slash | Yes | — |
| `ClientSecret`   | API client secret used to obtain a JWT       | Yes      | —             |
| `UserAgent`      | User-Agent header sent with every request    | No       | auto-detected |
| `TimeoutSeconds` | HTTP request timeout, in seconds (1–300)     | No       | 30            |

### Environment variables

Each property can also be supplied as an environment variable of the same name:

```bash
BaseUrl=https://your-shiftiq-instance.example.com/
ClientSecret=your-client-secret
TimeoutSeconds=60
```


## Project Structure

```
auth/bearer/
├── ApiClient.cs            HttpClient wrapper; bearer auth; JSON helpers
├── ApiResponse.cs          Response envelope with EnsureSuccess + GetJsonData<T>
├── Application.cs          Orchestrates the demos
├── ApplicationSettings.cs  Strongly-typed configuration
├── JsonSettings.cs         Shared Newtonsoft.Json serializer settings
├── Program.cs              Entry point: load config, fetch JWT, run demos
├── UserAgentGenerator.cs   Builds a sensible default User-Agent string
├── Demos/                  One file per endpoint family
├── Models/                 Plain DTOs that match the API JSON shape
└── Reports/                Render API data into human-readable text
```


## Usage

The client is async-first. Construct it once, fetch a JWT, then call any endpoint.

```csharp
using var client = new ApiClient(clientSecret, baseUrl, userAgent);

// Probe server health
var health = await client.GetAsync("api/diagnostic/health");
if (!health.Success)
    return;

// Exchange the client secret for a short-lived JWT, then use it for all subsequent requests
var tokenResponse = await client.PostAsync("api/security/tokens/generate", new { Secret = clientSecret });
tokenResponse.EnsureSuccess("POST", "api/security/tokens/generate");
var jwt = tokenResponse.GetJsonData<Jwt>();
client.UpdateClientSecret(jwt.AccessToken);

// Make a typed GET request
var response = await client.GetAsync("api/directory/groups");
response.EnsureSuccess("GET", "api/directory/groups");
var groups = response.GetJsonData<Group[]>();
```

> **Heads-up:** the JWT returned by `api/security/tokens/generate` is short-lived (see `Jwt.ExpiresInMinutes`). This quickstart fetches it once at startup and does not refresh it. Long-running callers must re-issue before expiry — otherwise subsequent requests will fail with HTTP 401.


## Examples

### Fetching and processing groups

```csharp
var response = await client.GetAsync("api/directory/groups");
response.EnsureSuccess("GET", "api/directory/groups");

var groups = response.GetJsonData<Group[]>()
    .OrderBy(x => x.GroupName)
    .ToArray();

foreach (var group in groups)
    Console.WriteLine($"  - {group.GroupName}");
```

### Filtering gradebooks by title

```csharp
var parameters = new Dictionary<string, string>
{
    ["GradebookTitle"] = "assess",
};

var response = await client.GetAsync("api/progress/gradebooks", parameters);
response.EnsureSuccess("GET", "api/progress/gradebooks");

var gradebooks = response.GetJsonData<Gradebook[]>()
    .OrderBy(x => x.GradebookTitle)
    .ToArray();
```

> **Use the `parameters` dictionary** rather than concatenating query strings. The client URL-encodes keys and values for you, so values containing spaces, `&`, or `=` are handled correctly.

### Paging through results

```csharp
var parameters = new Dictionary<string, string>
{
    ["filter.page"] = "1",
    ["filter.pagesize"] = "25",
};

var response = await client.GetAsync("api/progress/gradebooks", parameters);
response.EnsureSuccess("GET", "api/progress/gradebooks");

var items = response.GetJsonData<Gradebook[]>();

if (response.Headers.TryGetValue(Pagination.HeaderKey, out var paginationJson))
{
    var pagination = JsonConvert.DeserializeObject<Pagination>(paginationJson, JsonSettings.Default);
    Console.WriteLine($"Page {pagination.Page} of {pagination.CountPages()} ({pagination.TotalCount} items)");
}
```


## Building and Running

```bash
dotnet restore
dotnet build
dotnet run
```

You can pass an optional gradebook-title search term as the first argument:

```bash
dotnet run -- assess
```

### Visual Studio

1. Open `Quickstart.csproj`.
2. Create `appsettings.work.json` with your real values (see [Configuration](#configuration)).
3. Press **F5** to build and run.

---

> **Note**: This is a demo. Keep your real client secret in `appsettings.work.json` (git-ignored) or in environment variables — never in `appsettings.json`.
