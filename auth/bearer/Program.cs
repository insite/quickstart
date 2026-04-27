using System;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

namespace BearerAuthDemo
{
    public class Program
    {
        public static void WriteLine(string value)
            => Console.WriteLine(value + Environment.NewLine);

        public static async Task<int> Main(string[] args)
        {
            var settings = LoadAppSettings();

            WriteLine("User Agent: " + settings.UserAgent);

            using (var client = new ApiClient(settings.ClientSecret, settings.BaseUrl, settings.UserAgent, settings.TimeoutSeconds))
            {
                var health = await GetHealthAsync(client);
                if (health == null)
                {
                    WriteLine("Sorry, the API is currently offline.");
                    return 1;
                }

                WriteLine(health);

                var version = await GetVersionAsync(client);
                if (!string.IsNullOrEmpty(version))
                    WriteLine("API version: " + version);

                await ConfigureAccessTokenAsync(client, settings.ClientSecret);

                var basicGradebookSearchCriteria = args.Length == 1 ? args[0] : string.Empty;

                var app = new Application(client, basicGradebookSearchCriteria);

                await app.RunAsync(WriteLine);
            }

            return 0;
        }

        private static async Task<string> GetHealthAsync(ApiClient client)
        {
            var response = await client.GetAsync("api/diagnostic/health");

            return response.Success ? response.Data : null;
        }

        private static async Task<string> GetVersionAsync(ApiClient client)
        {
            var response = await client.GetAsync("api/diagnostic/version");

            return response.Success ? response.Data : null;
        }

        private static async Task<Jwt> RetrieveJwtAsync(ApiClient client, string secret)
        {
            const string url = "api/security/tokens/generate";

            var data = new { Secret = secret, Lifetime = 3600 };

            var response = await client.PostAsync(url, data);

            response.EnsureSuccess("POST", url);

            return JsonConvert.DeserializeObject<Jwt>(response.Data, JsonSettings.Default);
        }

        // The bearer token returned here is short-lived (see Jwt.ExpiresInMinutes). This quickstart
        // retrieves it once at startup and does not refresh it. Long-running callers must re-issue
        // before expiry — otherwise subsequent requests will fail with HTTP 401.
        //
        // Where a refresh would go: schedule a timer for ~80% of jwt.ExpiresIn seconds, then call
        // RetrieveJwtAsync(client, secret) again and pass the new AccessToken to
        // client.UpdateClientSecret(...). Alternatively, on a 401 response, refresh once and retry.
        private static async Task ConfigureAccessTokenAsync(ApiClient client, string secret)
        {
            var jwt = await RetrieveJwtAsync(client, secret);

            WriteLine($"{jwt.TokenType} access token generated.");
            WriteLine($"Token lifetime: {jwt.ExpiresIn} seconds ({jwt.ExpiresInMinutes} minutes). Expires at {DateTimeOffset.UtcNow.AddSeconds(jwt.ExpiresIn):u}.");

            client.UpdateClientSecret(jwt.AccessToken);
        }

        private static ApplicationSettings LoadAppSettings()
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.work.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .Build();

            var settings = new ApplicationSettings();

            configuration.Bind(settings);

            ValidateSettings(settings);

            if (string.IsNullOrEmpty(settings.UserAgent))
                settings.UserAgent = UserAgentGenerator.Generate();

            if (settings.TimeoutSeconds < 1 || settings.TimeoutSeconds > 300)
                settings.TimeoutSeconds = 30;

            return settings;
        }

        private static void ValidateSettings(ApplicationSettings settings)
        {
            var problems = new System.Collections.Generic.List<string>();

            if (string.IsNullOrWhiteSpace(settings.BaseUrl) || settings.BaseUrl.Contains("YOUR "))
                problems.Add("BaseUrl is missing or still set to the placeholder.");

            if (string.IsNullOrWhiteSpace(settings.ClientSecret) || settings.ClientSecret.Contains("YOUR "))
                problems.Add("ClientSecret is missing or still set to the placeholder.");

            if (problems.Count == 0)
                return;

            throw new InvalidOperationException(
                "Configuration is incomplete. Edit appsettings.json (or create appsettings.work.json) with real values.\n  - "
                + string.Join("\n  - ", problems));
        }
    }
}
