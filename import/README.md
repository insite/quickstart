# Directory API — How to import pending contact people

This is a .NET 10 console application that demonstrates every endpoint exposed by the **Pending Person**
controller in the Directory API (`/api/directory/pending-people`).

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- A valid Shift iQ instance URL and a **client secret** whose issued token satisfies these policies:
  - `directory/pending-people:data:1` — read (assert, retrieve, collect, count, search, download)
  - `directory/people:data:2` — modify
  - `directory/people:data:4` — create
  - `directory/people:data:8` — delete

## Setup

Configuration is loaded from two JSON files at startup:

| File                    | Tracked in git? | Purpose                                              |
|-------------------------|-----------------|------------------------------------------------------|
| `appsettings.json`      | yes             | Committed defaults / placeholders for the project    |
| `appsettings.work.json` | no (gitignored) | Your local values — base URL and client secret       |

`appsettings.work.json` is layered on top of `appsettings.json`, so any keys you set in the work
file override the committed defaults. A starter `appsettings.work.json` is included; open it and
fill in the two values:

```json
{
  "BaseUrl": "https://your-shift-instance.example.com/",
  "ClientSecret": "paste-your-client-secret-here"
}
```

Because `appsettings.work.json` is gitignored, your secret never gets committed back to the repo —
that is the practice we want integrators to copy into their own apps. Production deployments should
source these values from environment variables, a secret manager, or another configuration provider
rather than from a checked-in file.

On startup the program exchanges the client secret for a short-lived JWT by posting to
`api/security/tokens/generate`, and then uses the returned `accessToken` as the bearer credential
for every subsequent call. This is the recommended pattern — the long-lived secret never leaves the
initial handshake, and the token in flight expires on its own.

## Run

```bash
dotnet run
```

## What it covers

| # | Operation | HTTP Method | Path                                      |
|---|-----------|-------------|-------------------------------------------|
| 1 | Create    | POST        | `api/directory/pending-people`            |
| 2 | Assert    | HEAD        | `api/directory/pending-people/{id}`       |
| 3 | Retrieve  | GET         | `api/directory/pending-people/{id}`       |
| 4 | Modify    | PUT         | `api/directory/pending-people/{id}`       |
| 5 | Count     | POST        | `api/directory/pending-people/count`      |
| 6 | Collect   | POST        | `api/directory/pending-people/collect`    |
| 7 | Search    | POST        | `api/directory/pending-people/search`     |
| 8 | Download  | POST        | `api/directory/pending-people/download`   |
| 9 | Delete    | DELETE      | `api/directory/pending-people/{id}`       |

## Paging, sorting, and export format

Collect, count, search, and download accept an optional `QueryFilter` that is serialized onto the
URL as query-string parameters:

```
?page=1&pageSize=20&sort=UserLastName+asc,SubmittedAt+desc
?format=csv       (download only)
```

The request body carries the filtering criteria (`CollectPendingPeople` / `CountPendingPeople` /
`SearchPendingPeople`):

```json
{
  "userLastName": "Smith",
  "submittedSince": "2026-01-01T00:00:00Z"
}
```

## Project structure

| File                      | Purpose                                            |
|---------------------------|----------------------------------------------------|
| `Program.cs`              | Entry point — walks through every endpoint         |
| `PendingPersonClient.cs`  | Typed HTTP client wrapping the API                 |
| `Models.cs`               | Request / response DTOs matching the API spec      |
