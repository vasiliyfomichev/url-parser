using System.Web;
using System.Web.Mvc;
using log4net;
using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;
using URL_Parser.Properties;

namespace URL_Parser.Configuration.Filters
{
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        #region Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        #endregion

        public override void OnException(HttpActionExecutedContext context)
        {
            Logger.Error(Resources.UnhandledExceptionError, context.Exception);
            if (context.Exception is WebException || context.Exception is HttpRequestException)
            {
                context.Response =
                    new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            else
            {
                context.Response =
                    new HttpResponseMessage(HttpStatusCode.ServiceUnavailable);
                var httpUnhandledException = new HttpUnhandledException(context.Exception.Message, context.Exception);
                ErrorNotifier.EmailError(httpUnhandledException.GetHtmlErrorMessage());
            }
        }
    }
}