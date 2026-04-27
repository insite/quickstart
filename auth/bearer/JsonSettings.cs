using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace BearerAuthDemo;

/// <summary>
/// Shared Newtonsoft.Json settings used for every request and response.
/// Case-insensitive matching is on by default in Newtonsoft, so a JSON property
/// like "gradebookTitle" or "GRADEBOOKTITLE" will still bind to GradebookTitle.
/// </summary>
public static class JsonSettings
{
    public static readonly JsonSerializerSettings Default = new()
    {
        ContractResolver = new DefaultContractResolver(),
        NullValueHandling = NullValueHandling.Ignore,
    };
}
