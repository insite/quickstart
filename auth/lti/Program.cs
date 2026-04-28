using LtiLaunch.Api.Lti;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.work.json", optional: true, reloadOnChange: true);

builder.Services.Configure<LtiDefaults>(builder.Configuration.GetSection("LtiDefaults"));
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
    options.SerializerOptions.DictionaryKeyPolicy = null;
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapGet("/api/lti/defaults", (IOptions<LtiDefaults> options) => options.Value);

app.MapPost("/api/lti/ticket", (TicketRequest request) =>
{
    if (string.IsNullOrWhiteSpace(request.LaunchUrl) || !Uri.TryCreate(request.LaunchUrl, UriKind.Absolute, out var url))
        return Results.BadRequest(new { error = "LaunchUrl must be an absolute URL." });

    if (string.IsNullOrWhiteSpace(request.OrganizationSecret))
        return Results.BadRequest(new { error = "OrganizationSecret is required." });

    var parameters = new LtiParameters("POST");
    parameters.Add("oauth_consumer_key", request.OrganizationSecret);
    parameters.Add("user_id", request.LearnerCode);
    parameters.Add("lis_person_name_given", request.LearnerNameFirst);
    parameters.Add("lis_person_name_family", request.LearnerNameLast);
    parameters.Add("lis_person_contact_email_primary", request.LearnerEmail);
    parameters.Add("roles", LtiRole.Learner);
    parameters.Add("shift_group_name", request.GroupName);
    parameters.Add("shift_organization_identifier", request.OrganizationIdentifier);

    var ticket = LtiTicketHelper.GetTicket(request.OrganizationSecret, url, parameters);
    ticket.Parameters["oauth_signature"] = ticket.Signature;

    var sorted = ticket.Parameters
        .OrderBy(pair => pair.Key, StringComparer.Ordinal)
        .ToDictionary(pair => pair.Key, pair => pair.Value);

    return Results.Ok(new TicketResponse(ticket.Url.ToString(), sorted));
});

app.Run();
