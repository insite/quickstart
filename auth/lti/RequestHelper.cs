namespace LtiLaunchDemo
{
    public static class RequestHelper
    {
        public static string GetDomainName(string domain)
        {
            var tokens = domain.Split('.');

            if (tokens.Length > 2)
                domain = string.Join(".", tokens, 1, tokens.Length - 1);

            return domain;
        }

        public static string GetDomainName(System.Web.UI.Page page)
        {
            string domain = page.Request.Url.DnsSafeHost.ToLower();

            return GetDomainName(domain);
        }
    }
}