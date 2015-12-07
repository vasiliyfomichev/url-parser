#region

using System.Linq;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using URL_Parser.Tests.Properties;
using URL_Parser.Utility;

#endregion

namespace URL_Parser.Tests
{
    [TestClass]
    public class UrlUtitlityTest
    {
        private const string EmptyHtmlDocument =
            "<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\">" +
            "<head><meta charset=\"utf-8\" /><title></title></head><body></body></html>";

        #region IsImageUrl

        [TestMethod, TestCategory("Unit")]
        public void Bad_Image_URLs_Are_Properly_Detected()
        {
            Assert.IsFalse(UrlUtil.IsImageUrl("http://somedomain/asdasdasdasdadsasd.com"));
            Assert.IsFalse(UrlUtil.IsImageUrl("asdasdasdasdad.png.com"));
            Assert.IsFalse(UrlUtil.IsImageUrl("asdasdasdasdad.png.png1"));
            Assert.IsFalse(UrlUtil.IsImageUrl("asdasdasdasdad.png.123gif"));  
        }

        [TestMethod, TestCategory("Unit")]
        public void Good_Image_URLs_Are_Properly_Detected()
        {
            // allowed image extensions: jpg|gif|jpeg|png|ico
            Assert.IsTrue(UrlUtil.IsImageUrl("http://somedomain/asdasdasdasdadsasd.png"));
            Assert.IsTrue(UrlUtil.IsImageUrl("asdasdasdasdad.png"));
            Assert.IsTrue(UrlUtil.IsImageUrl("/asdasdasdasdad.png"));
            Assert.IsTrue(UrlUtil.IsImageUrl("../../asdasdasdasdad.png"));
            Assert.IsTrue(UrlUtil.IsImageUrl("../../asdasdasdasdad.gif"));
            Assert.IsTrue(UrlUtil.IsImageUrl("../../asdasdasdasdad.jpeg"));
            Assert.IsTrue(UrlUtil.IsImageUrl("../../asdasdasdasdad.ico"));
        }

        #endregion

        #region IsUri

        [TestMethod, TestCategory("Unit")]
        public void Bad_URIs_Are_Properly_Detected()
        {
            Assert.IsFalse(UrlUtil.IsUri("asass"));
            Assert.IsFalse(UrlUtil.IsUri("www."));
            Assert.IsFalse(UrlUtil.IsUri("asd.com"));
            Assert.IsFalse(UrlUtil.IsUri("www.adad.com"));
        }

        [TestMethod, TestCategory("Unit")]
        public void Good_URIs_Are_Properly_Detected()
        {
            Assert.IsTrue(UrlUtil.IsUri("httsp://asd.com"));
            Assert.IsTrue(UrlUtil.IsUri("http://www.adad.com"));
        }

        #endregion

        #region IsUri

        [TestMethod, TestCategory("Unit")]
        public void Bad_Absolute_URIs_Are_Properly_Detected()
        {
            Assert.IsFalse(UrlUtil.IsUri("asass"));
            Assert.IsFalse(UrlUtil.IsUri("www."));
            Assert.IsFalse(UrlUtil.IsUri("asd.com"));
            Assert.IsFalse(UrlUtil.IsUri("www.adad.com"));
        }

        [TestMethod, TestCategory("Unit")]
        public void Good_Absolute_URIs_Are_Properly_Detected()
        {
            Assert.IsTrue(UrlUtil.IsUri("httsp://www.asd.com"));
        }

        #endregion

        #region IsUri

        [TestMethod, TestCategory("Unit")]
        public async void Ensure_UrlExistsAsync_Properly_Detects_Bad_URLs()
        {
            Assert.IsFalse(await UrlUtil.UrlExistsAsync("http://adasda"));
            Assert.IsFalse(await UrlUtil.UrlExistsAsync("http://adasda"));
            Assert.IsFalse(await UrlUtil.UrlExistsAsync(string.Empty));
        }

        [TestMethod, TestCategory("Integration")]
        public async void Ensure_UrlExistsAsync_Properly_Detects_Good_URLs()
        {
            Assert.IsFalse(await UrlUtil.UrlExistsAsync("http://google.com"));
            Assert.IsFalse(await UrlUtil.UrlExistsAsync("http://www.google.com"));
        }

        #endregion

        [TestMethod, TestCategory("Integration") ]
        public void Ensure_GetUrlContent_Properly_Downlods_Content()
        {
            Assert.AreEqual(EmptyHtmlDocument, UrlUtil.GetUrlContent(Settings.Default.EmptyHtmlPagePath));
        }

        [TestMethod, TestCategory("Integration")]
        public void Ensure_GetMetaImageUrls_Properly_Gets_Image_URLs_From_Meta_Tags()
        {
            var document = new HtmlWeb().Load(Settings.Default.TestHtmlPagePath);
            var metaImages = UrlUtil.GetMetaImageUrls(document);
            Assert.AreEqual(2, metaImages.Count());
            Assert.AreEqual("https://www.cmsbestpractices.com/wp-content/uploads/2013/12/favicon1.ico", metaImages.First().Src);
        }

        [TestMethod, TestCategory("Integration")]
        public void Ensure_GetCssFilePaths_Properly_Gets_CSS_Paths_Paths_From_Head_and_Body()
        {
            var document = new HtmlWeb().Load(Settings.Default.TestHtmlPagePath);
            var cssPaths = UrlUtil.GetCssFilePaths(document);
            Assert.AreEqual(2, cssPaths.Count());
            Assert.AreEqual(Settings.Default.TestCssFilePath,
                cssPaths.First());
        }

        [TestMethod, TestCategory("Integration")]
        public void Ensure_GetScriptFilePaths_Properly_Gets_JS_Paths_From_Head_and_Body()
        {
            var document = new HtmlWeb().Load(Settings.Default.TestHtmlPagePath);
            var jsPaths = UrlUtil.GetScriptFilePaths(document);
            Assert.AreEqual(2, jsPaths.Count());
            Assert.AreEqual(Settings.Default.TestJsFilePath,
                jsPaths.First());
        }

        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetScriptFilePaths_Properly_Null_When_There_Are_No_Scripts_Referenced()
        {
            var document = new HtmlDocument();
            document.LoadHtml(EmptyHtmlDocument);
            var jsPaths = UrlUtil.GetScriptFilePaths(document);
            Assert.IsNull(jsPaths);
        }
    }
}
