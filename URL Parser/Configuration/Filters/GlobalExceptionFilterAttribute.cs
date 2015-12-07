#region

using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using log4net;
using URL_Parser.Properties;

#endregion

namespace URL_Parser.Configuration.Filters
{
    /// <summary>
    /// Catch all exception handler.
    /// </summary>
    public class GlobalExceptionFilterAttribute : ExceptionFilterAttribute
    {
        #region Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        #endregion

        public override void OnException(HttpActionExecutedContext context)
        {
            Logger.Error(Resources.UnhandledExceptionError, context.Exception);
            if (context.Exception is GenericException || (context.Exception is AggregateException && context.Exception.InnerException is GenericException))
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