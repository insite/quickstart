# LTI Authentication Demo

This repository contains the source code for a demonstration of Single Sign On (SSO) using Learning Tools Interoperability (LTI).

The integration endpoint for an LTI Launch in Shift iQ is platform-agnostic, so it can be used to integrate any application that is able to send an HTTP request.

This particular demo is split into two pieces:

- A **static HTML/CSS/JS website** in `wwwroot/` that collects the LTI parameters and posts the signed launch request to Shift iQ.
- A **.NET 10 minimal API** at the project root that signs the LTI launch request server-side using HMAC-SHA256.

You can use the source code here as a reference for building an integration from any web application written in any programming language.

## Project layout

```
.
├── LtiLaunch.Api.csproj      .NET 10 API project file
├── LtiLaunch.sln             Solution file
├── Program.cs                Minimal API host and endpoint definitions
├── appsettings.json          Default LTI parameter values (under "LtiDefaults")
├── appsettings.work.json     Local-only overrides for private values (gitignored)
├── Properties/
│   └── launchSettings.json
├── Lti/                      LTI signing logic
│   ├── LtiParameters.cs
│   ├── LtiRole.cs
│   ├── LtiTicket.cs
│   ├── LtiTicketHelper.cs
│   └── TicketModels.cs
└── wwwroot/                  Static frontend (deployable as-is to any static host)
    ├── index.html
    ├── site.css
    └── site.js
```

## API

| Method | Path                  | Purpose                                                                                  |
| ------ | --------------------- | ---------------------------------------------------------------------------------------- |
| GET    | `/api/lti/defaults`   | Returns the default form values from `appsettings.json` so the page can pre-populate.    |
| POST   | `/api/lti/ticket`     | Accepts JSON form values, returns the signed LTI parameters and the destination URL.    |

`POST /api/lti/ticket` request body:

```json
{
  "LearnerCode": "BB123",
  "LearnerEmail": "bugs.bunny@example.com",
  "LearnerNameFirst": "Bugs",
  "LearnerNameLast": "Bunny",
  "GroupName": "Tunes",
  "OrganizationIdentifier": "f85f5022-...",
  "OrganizationSecret": "bB8u6Tj6...",
  "LaunchUrl": "https://dev-demo.shiftiq.com/ui/lobby/integration/lti/launch"
}
```

Response:

```json
{
  "url": "https://dev-demo.shiftiq.com/ui/lobby/integration/lti/launch",
  "parameters": {
    "lis_person_contact_email_primary": "bugs.bunny@example.com",
    "lti_message_type": "basic-lti-launch-request",
    "oauth_consumer_key": "...",
    "oauth_nonce": "...",
    "oauth_signature": "...",
    "oauth_signature_method": "HMAC-SHA256",
    "oauth_timestamp": "...",
    "oauth_version": "1.0",
    "...": "..."
  }
}
```

## Running locally

You will need the [.NET 10 SDK](https://dotnet.microsoft.com/download).

Create `appsettings.work.json` (gitignored) alongside `appsettings.json` and add the private values for your local environment:

```json
{
  "LtiDefaults": {
    "OrganizationIdentifier": "...",
    "OrganizationSecret": "...",
    "LaunchUrl": "https://dev-demo.shiftiq.com/ui/lobby/integration/lti/launch"
  }
}
```

Then run:

```
dotnet run
```

Then open `http://localhost:5005`. Fill in the LTI Launch Request form (you will need an Organization Identifier and an Organization Secret from the support team), click **Validate**, then click **Launch** to post the signed request to the Shift iQ LTI endpoint.

If your request is valid, then you will be authenticated automatically by Shift iQ. If the learner identified in the request does not already exist in Shift iQ, then a new account for the learner is created and approved automatically.

## Deploying the static site separately

The contents of `wwwroot/` have no server-side dependencies and can be hosted on any static file host (Azure Static Web Apps, S3 + CloudFront, GitHub Pages, etc.). If you do this, deploy the API separately and update `site.js` to point its `fetch` calls at the API's absolute URL — and add a CORS policy on the API to allow the static site's origin.
