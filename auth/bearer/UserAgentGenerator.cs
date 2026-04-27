using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace BearerAuthDemo;

public static class UserAgentGenerator
{
    /// <summary>
    /// Generates a User-Agent string following standard conventions
    /// </summary>
    /// <param name="applicationName">Your application name (defaults to assembly name)</param>
    /// <param name="includeFrameworkInfo">Whether to include .NET framework information</param>
    /// <returns>Formatted User-Agent string</returns>
    public static string Generate(string applicationName = null, bool includeFrameworkInfo = true)
    {
        // Get application name and version
        var assembly = Assembly.GetEntryAssembly() ?? Assembly.GetExecutingAssembly();
        var appName = applicationName ?? assembly.GetName().Name ?? "DotNetApp";
        var version = assembly.GetName().Version?.ToString() ?? "1.0.0";

        // Build the base product string
        var userAgent = $"{appName}/{version}";

        // Get platform information
        var platformInfo = GetPlatformInfo();
        if (!string.IsNullOrEmpty(platformInfo))
            userAgent += $" ({platformInfo})";

        // Optionally add .NET runtime information
        if (includeFrameworkInfo)
        {
            var frameworkInfo = GetFrameworkInfo();
            if (!string.IsNullOrEmpty(frameworkInfo))
                userAgent += $" {frameworkInfo}";
        }

        return userAgent;
    }

    /// <summary>
    /// Gets platform and architecture information
    /// </summary>
    private static string GetPlatformInfo()
    {
        var platform = GetPlatformName();
        var architecture = RuntimeInformation.ProcessArchitecture.ToString();

        return $"{platform}; {architecture}";
    }

    /// <summary>
    /// Gets the platform name in standard format
    /// </summary>
    private static string GetPlatformName()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Try to get Windows version info
            var osVersion = Environment.OSVersion;
            if (osVersion.Platform == PlatformID.Win32NT)
            {
                return $"Windows NT {osVersion.Version.Major}.{osVersion.Version.Minor}";
            }
            return "Windows";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // Check if running in container
            if (IsRunningInContainer())
            {
                return "Linux; Docker";
            }
            return "Linux";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            return "macOS";
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD))
        {
            return "FreeBSD";
        }

        return "Unknown";
    }

    /// <summary>
    /// Gets .NET framework information
    /// </summary>
    private static string GetFrameworkInfo()
    {
        var frameworkDescription = RuntimeInformation.FrameworkDescription;

        // Clean up the framework description for User-Agent format
        // ".NET 9.0.0" -> "NET/9.0.0"
        if (frameworkDescription.StartsWith(".NET "))
        {
            var version = frameworkDescription.Substring(5);
            return $"NET/{version}";
        }

        return frameworkDescription.Replace(" ", "").Replace(".", "/");
    }

    /// <summary>
    /// Checks if the application is running in a container
    /// </summary>
    private static bool IsRunningInContainer()
    {
        try
        {
            // Common indicators of container environment
            return Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true" ||
                   System.IO.File.Exists("/.dockerenv") ||
                   Environment.GetEnvironmentVariable("container") != null;
        }
        catch
        {
            return false;
        }
    }
}