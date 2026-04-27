# Shift iQ API Quickstart

Sample applications that demonstrate how to integrate with the Shift iQ API. Each folder is a self-contained project with its own README — pick the one that matches the integration you need.

> Applies to version 2 of the Shift iQ API. Full documentation: [docs.shiftiq.com/developers](https://docs.shiftiq.com/developers).

## Projects

| Path | Stack | What it shows |
|------|-------|---------------|
| [`auth/bearer`](auth/bearer/README.md) | .NET 10 console | Exchange a client secret for a JWT, then call groups, users, and gradebook endpoints |
| [`auth/cookie`](auth/cookie/README.md) | .NET 10 ASP.NET Core | Localhost web UI that walks through cookie-based auth against the API |
| [`auth/lti`](auth/lti/README.md) | ASP.NET (Web Forms) | LTI launch integration for SSO into Shift iQ |
| [`import`](import/README.md) | .NET 10 console | Every endpoint on the Pending Person controller (`/api/directory/pending-people`) |

The Bearer Authentication demo is the recommended starting point.

## Credentials

Each .NET project loads configuration from `appsettings.json` (committed placeholders) layered with `appsettings.work.json` (git-ignored, holds your real `BaseUrl` and `ClientSecret`). Environment variables override both. See the per-project README for the exact keys.

## License

Released under the Unlicense — see [LICENSE](LICENSE).
