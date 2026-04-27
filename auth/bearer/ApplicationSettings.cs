namespace BearerAuthDemo;

public class ApplicationSettings
{
    // Client secret (API key)
    public string ClientSecret { get; set; } = string.Empty;

    // Fully-qualified URL ending with forward slash
    public string BaseUrl { get; set; } = string.Empty;

    // ProductName/Version (SystemInfo; PlatformInfo; RuntimeInfo)
    public string UserAgent { get; set; } = string.Empty;

    // Default timeout for HTTP requests submitted to the API server
    public int TimeoutSeconds { get; set; } = 30;
}
