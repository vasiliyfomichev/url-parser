#region

using System.IO;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using URL_Parser.Tests.Properties;

#endregion

namespace URL_Parser.Tests
{
    class Util
    {
        public static HttpContext FakeHttpContext()
        {
            var httpRequest = new HttpRequest(string.Empty, Settings.Default.TestRequestUrl, string.Empty);
            var stringWriter = new StringWriter();
            var httpResponse = new HttpResponse(stringWriter);
            var httpContext = new HttpContext(httpRequest, httpResponse);

            var sessionContainer = new HttpSessionStateContainer("id", new SessionStateItemCollection(),
                                                    new HttpStaticObjectsCollection(), 10, true,
                                                    HttpCookieMode.AutoDetect,
                                                    SessionStateMode.InProc, false);

            httpContext.Items["AspSession"] = typeof(HttpSessionState).GetConstructor(
                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                        null, CallingConventions.Standard,
                                        new[] { typeof(HttpSessionStateContainer) },
                                        null)
                                .Invoke(new object[] { sessionContainer });

            return httpContext;
        }
    }
}
