#region

using System.Web.Http;
using URL_Parser.Configuration.Filters;

#endregion

namespace URL_Parser
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            config.Filters.Add(new UrlValidatorFilterAttribute());
            config.Filters.Add(new UrlFormatterFilterAttribute());
            config.Filters.Add(new GlobalExceptionFilterAttribute());
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
