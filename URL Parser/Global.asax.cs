#region

using System;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using log4net;
using URL_Parser.Configuration;

#endregion

namespace URL_Parser
{
    public class MvcApplication : HttpApplication
    {
        #region Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        #endregion

        public void Application_Start()
        {
            log4net.Config.XmlConfigurator.Configure();
            Logger.Info("Starting URL Parser Application.");
            AutofacConfig.ConfigureContainer();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        public void Application_End()
        {
            Logger.Info("URL Parser application is shutting down.");
        }

        void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();
            Logger.Error("Unhandled error in URL Parser.", ex);
            var httpUnhandledException = new HttpUnhandledException(Server.GetLastError().Message, Server.GetLastError());
            ErrorNotifier.EmailError(httpUnhandledException.GetHtmlErrorMessage());
            Server.ClearError();
        }
    }
}