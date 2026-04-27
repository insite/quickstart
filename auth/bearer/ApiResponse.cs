using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace BearerAuthDemo
{
    /// <summary>
    /// Response object containing all relevant HTTP response information.
    /// </summary>
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();
        public string Data { get; set; }
        public bool Success { get; set; }
        public string Error { get; set; }

        public T GetJsonData<T>()
        {
            if (string.IsNullOrEmpty(Data))
                return default;

            try
            {
                return JsonConvert.DeserializeObject<T>(Data, JsonSettings.Default);
            }
            catch (JsonException)
            {
                throw new InvalidOperationException("Response data is not valid JSON");
            }
        }

        /// <summary>
        /// Throws with status, error, and a body snippet when the request was not successful.
        /// Call this before deserializing <see cref="Data"/> to surface auth/path/server errors clearly.
        /// </summary>
        public ApiResponse EnsureSuccess(string method, string url)
        {
            if (Success)
                return this;

            var snippet = string.IsNullOrEmpty(Data)
                ? string.Empty
                : (Data.Length > 300 ? Data.Substring(0, 300) + "..." : Data);

            throw new InvalidOperationException(
                $"{method} {url} failed: HTTP {StatusCode}. {Error}{(string.IsNullOrEmpty(snippet) ? "" : $" Body: {snippet}")}");
        }
    }
}
