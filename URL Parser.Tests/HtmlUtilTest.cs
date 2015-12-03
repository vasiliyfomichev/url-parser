#region

using HtmlAgilityPack;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using URL_Parser.Utility;

#endregion

namespace URL_Parser.Tests
{
    [TestClass]
    public class HtmlUtilTest
    {
        [TestMethod, TestCategory("Unit")]
        public void Make_Sure_CleanupString_Removes_All_Numbers()
        {
            const string testString = "hello \t\n\r 2015 15 332 bananas";
            var actualString = HtmlUtil.CleanupString(testString);
            Assert.AreEqual("hello bananas", actualString);
        }

        [TestMethod, TestCategory("Unit")]
        public void Make_Sure_CleanupDocument_Removes_All_Script_Style_Comment_Elements()
        {
            var document = new HtmlDocument();
            document.LoadHtml(
                "hello<script>var someVar = document.getElementById(\"test\");var test = \"/some/image.gif\";" +
                "</script><style></style><!-- testing -->");
            var cleanDocument = HtmlUtil.CleanupDocument(document);
            Assert.AreEqual("hello", cleanDocument.DocumentNode.OuterHtml);
        }
    }
}