using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace URL_Parser.Filters
{
    public class UrlFormatter : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var url = actionContext.ActionArguments["url"] as string;
            url = UrlUtil.EnsureAbsoluteUrlFormat(url, HttpContext.Current);
            actionContext.ActionArguments["url"] = url;
        }
    }
}