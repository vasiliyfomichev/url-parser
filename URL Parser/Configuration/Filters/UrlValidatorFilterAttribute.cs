#region

using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using URL_Parser.Properties;
using URL_Parser.Utility;

#endregion

namespace URL_Parser.Configuration.Filters
{
    public class UrlValidatorFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var url = actionContext.ActionArguments["url"] as string;
            if (!string.IsNullOrWhiteSpace(url) && UrlUtil.UrlExistsAsync(url).GetAwaiter().GetResult())
                return;
            throw new HttpRequestException(Resources.UnableToGetHeaderError);
        }
    }
}