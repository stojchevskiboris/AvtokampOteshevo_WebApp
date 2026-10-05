using System.Web.Mvc;
using System.Web.Routing;

namespace Project_IT.Helpers
{
    public static class UrlExtensions
    {
        public static string ForCulture(this UrlHelper url, string culture)
        {
            if (url == null) return "/";
            var values = new RouteValueDictionary(url.RequestContext.RouteData.Values);
            values["culture"] = culture;

            var queryString = url.RequestContext.HttpContext.Request.QueryString;
            if (queryString != null)
            {
                foreach (string key in queryString.Keys)
                {
                    if (key != null && !values.ContainsKey(key))
                    {
                        values[key] = queryString[key];
                    }
                }
            }

            return url.RouteUrl(values) ?? ("/" + culture + "/");
        }
    }
}
