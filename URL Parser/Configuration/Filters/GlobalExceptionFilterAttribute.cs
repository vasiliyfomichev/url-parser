using System.Web;
using log4net;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace URL_Parser.Configuration.Filters
{
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        #region Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        #endregion

        public override void OnException(HttpActionExecutedContext context)
        {
            Logger.Error("Unhandled exception in Web API component of URL Parser.", context.Exception);
            context.Response =
                new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
            var httpUnhandledException = new HttpUnhandledException(context.Exception.Message, context.Exception);
            ErrorNotifier.EmailError(httpUnhandledException.GetHtmlErrorMessage());
        }
    }
}