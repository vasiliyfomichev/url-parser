#region

using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using URL_Parser.Utility;

#endregion

namespace URL_Parser.Configuration.Filters
{
    /// <summary>
    /// Formats the URL passed to the service for processing.
    /// </summary>
    public class UrlFormatterFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var url = actionContext.ActionArguments["url"] as string;
            url = HttpUtility.HtmlDecode(url);
            url = UrlUtil.EnsureAbsoluteUrlFormat(url, HttpContext.Current);
            actionContext.ActionArguments["url"] = url;
        }
    }
}