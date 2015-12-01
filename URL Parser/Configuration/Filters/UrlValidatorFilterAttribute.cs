#region

using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using URL_Parser.Utility;

#endregion

namespace URL_Parser.Configuration.Filters
{
    public class UrlValidatorFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var url = actionContext.ActionArguments["url"] as string;
            if (!string.IsNullOrWhiteSpace(url) && UrlUtil.UrlExists(url))
                return;
            actionContext.Response = actionContext.Request.CreateErrorResponse(
                HttpStatusCode.BadRequest, actionContext.ModelState);
        }
    }
}