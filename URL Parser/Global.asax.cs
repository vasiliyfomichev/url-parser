#region

using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using URL_Parser.App_Start;

#endregion

namespace URL_Parser
{
    public class MvcApplication : HttpApplication
    {
        public void Application_Start()
        {
            AutofacConfig.ConfigureContainer();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }
    }
}