#region

using System.Linq;
using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using URL_Parser.Utility;

#endregion

namespace URL_Parser.Tests
{
    [TestClass]
    public class ImageUtilTest
    {
        [TestMethod, TestCategory("Integration")]
        public void Ensure_Images_Are_Properly_Found_In_Image_Tags()
        {
            var document = new HtmlWeb().Load("http://cmsbestpractices.com/test.html");
            var images = ImageUtil.GetImagesFromImageTags(document, "http://cmsbestpractices.com");
            Assert.AreEqual(1, images.Count()); 
            Assert.AreEqual("https://www.cmsbestpractices.com/wp-content/uploads/2013/12/CMS-Best-Practices-Logo1.png", images.First().Src);
        }
    }
}
