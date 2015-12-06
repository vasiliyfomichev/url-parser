#region

using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using URL_Parser.Properties;
using URL_Parser.Utility;

#endregion

namespace URL_Parser.Configuration.Filters
{
    /// <summary>
    /// Validates the URL passed in for parsing.
    /// </summary>
    public class UrlValidatorFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            var url = actionContext.ActionArguments["url"] as string;
            if (!string.IsNullOrWhiteSpace(url) && UrlUtil.UrlExistsAsync(url).GetAwaiter().GetResult())
                return;
            throw new GenericException(Resources.UnableToGetHeaderError);
        }
    }
}