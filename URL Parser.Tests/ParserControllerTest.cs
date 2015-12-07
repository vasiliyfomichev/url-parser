#region

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using URL_Parser.Controllers;
using URL_Parser.Models;
using URL_Parser.Providers;
using URL_Parser.Tests.Properties;

#endregion

namespace URL_Parser.Tests
{
    [TestClass]
    public class ParserControllerTest
    {
        private readonly ILog _logger = LogManager.GetLogger(typeof(MvcApplication));


        [TestMethod, TestCategory("Integration")]
        public void Ensure_ParserController_Properly_Retrieves_All_Images()
        {
            var controller = new ParserController(new UrlService(_logger), _logger)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            HttpContext.Current = Util.FakeHttpContext();
            var images = controller.Images(Settings.Default.TestHtmlPagePath) as OkNegotiatedContentResult<IEnumerable<Image>>;
            Assert.AreEqual(6, images.Content.Count());
        }

        [TestMethod, TestCategory("Integration")]
        public void Ensure_ImageController_Properly_Retrieves_WordReport()
        {
            var controller = new ParserController(new UrlService(_logger), _logger)
            {
                Request = new HttpRequestMessage(),
                Configuration = new HttpConfiguration()
            };

            HttpContext.Current = Util.FakeHttpContext();
            var reportItems = controller.WordReport(Settings.Default.TestHtmlPagePath) as OkNegotiatedContentResult<IEnumerable<WordReportItem>>;
            Assert.AreEqual(3, reportItems.Content.Count());
        }
    }
}
