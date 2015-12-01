#region

using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using URL_Parser.Utility;

#endregion

namespace URL_Parser.Configuration.Filters
{
    public class UrlFormatterFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var url = actionContext.ActionArguments["url"] as string;
            url = UrlUtil.EnsureAbsoluteUrlFormat(url, HttpContext.Current);
            actionContext.ActionArguments["url"] = url;
        }
    }
}