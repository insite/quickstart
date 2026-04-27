using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;

namespace LtiLaunchDemo
{
    public class LtiParameters
    {
        public string HttpMethod { get; private set; }

        private readonly NameValueCollection _parameters;

        public LtiParameters(string httpMethod)
        {
            HttpMethod = httpMethod;

            _parameters = new NameValueCollection
            {
                { "oauth_callback", "about:blank" },
                { "oauth_nonce", Guid.NewGuid().ToString("N") },
                { "oauth_signature_method", "HMAC-SHA256" },
                { "oauth_timestamp", ToUnixTimestamp(DateTime.UtcNow).ToString() },
                { "oauth_version", "1.0" },

                { "launch_presentation_locale", CultureInfo.CurrentCulture.Name },
                { "lti_message_type", "basic-lti-launch-request" },
                { "lti_version", "LTI-1p0" }
            };
        }

        public void Add(string name, string value)
        {
            _parameters.Add(name, value);
        }

        public void Add(string name, params LtiRole[] roles)
        {
            var value = roles.Any() ? string.Join(",", roles.ToList()) : string.Empty;

            _parameters.Add(name, value);
        }

        public bool Contains(string name)
        {
            string value = _parameters[name];
            return value != null;
        }

        public NameValueCollection GetParameters()
        {
            return new NameValueCollection(_parameters);
        }

        public static long ToUnixTimestamp(DateTime value)
        {
            var _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return (long)(value.ToUniversalTime() - _unixEpoch).TotalSeconds;
        }
    }
}