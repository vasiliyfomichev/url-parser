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
    public class ImageUtilTest
    {
        [TestMethod, TestCategory("Integration")]
        public void Ensure_GetImagesFromImageTags_Properly_Finds_Images()
        {
            var document = new HtmlWeb().Load(Settings.Default.TestHtmlPagePath);
            var images = ImageUtil.GetImagesFromImageTags(document, Settings.Default.TestRequestUrl);
            Assert.AreEqual(1, images.Count()); 
            Assert.AreEqual("https://www.cmsbestpractices.com/wp-content/uploads/2013/12/CMS-Best-Practices-Logo1.png", images.First().Src);
        }

        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetImagesFromInlineCss_Properly_Finds_Images()
        {
            var document = new HtmlDocument();
            document.LoadHtml("<style>.test{height:12px; background:url('asdasd/asdasd/image1.jpg');" +
                              "background:url(\"asdasd/asdasd/image2.jpg\")</style>");

            var images = ImageUtil.GetImagesFromInlineCss(document, "http://cmsbestpractices.com/a/b/styles.css");
            Assert.AreEqual(2, images.Count());
        }

        [TestMethod, TestCategory("Unit")]
        public void Ensure_GetImagesFromInlineJs_Properly_Finds_Images()
        {
            var document = new HtmlDocument();
            document.LoadHtml("<script>var someVar = document.getElementById(\"test\");var test = \"/some/image.gif\";</script>");
            var images = ImageUtil.GetImagesFromInlineJs(document, Settings.Default.TestRequestUrl);
            Assert.AreEqual(1, images.Count());
            Assert.AreEqual("http://cmsbestpractices.com/some/image.gif", images.First().Src);
        }

        [TestMethod, TestCategory("Integration")]
        public void Ensure_GetImagesFromScriptFile_Properly_Finds_Images_In_Referenced_Script_File()
        {
            var images = ImageUtil.GetImagesFromScriptFile(Settings.Default.TestJsFilePath, Util.FakeHttpContext());
            Assert.AreEqual(1, images.Count());
        }

        [TestMethod, TestCategory("Integration")]
        public void Ensure_GetImagesFromCssFile_Properly_Finds_Images_In_Referenced_Style_File()
        {
            var images = ImageUtil.GetImagesFromCssFile(Settings.Default.TestCssFilePath, Util.FakeHttpContext());
            Assert.AreEqual(2, images.Count());
        }
    }
}
