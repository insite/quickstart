using System;

namespace BearerAuthDemo;

public class Export
{
    public string DownloadUrl { get; set; }
    public string ExportFormat { get; set; }
    public string ExportKey { get; set; }
    public DateTime Expiry { get; set; }
}