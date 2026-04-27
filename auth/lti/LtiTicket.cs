using System;
using System.Collections.Specialized;

namespace LtiLaunchDemo
{
    public class LtiTicket
    {
        public Uri Url { get; private set; }

        public string Signature { get; private set; }

        public NameValueCollection Parameters { get; private set; }

        public LtiTicket(Uri url, string signature, NameValueCollection parameters)
        {
            Url = url;
            Signature = signature;
            Parameters = parameters;
        }
    }
}