#region

using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using log4net;
using URL_Parser.Properties;

#endregion

namespace URL_Parser.Utility
{
    public class HtmlUtil
    {
        #region Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        #endregion

        public static string CleanupString(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            Logger.DebugFormat(Resources.TextToCleanMessage, text);
            text = Regex.Replace(text, Settings.Default.NewLineRegex, string.Empty).Trim();
            text = HttpUtility.HtmlDecode(text);
            text = Regex.Replace(text, @"[^\u0000-\u007F]", string.Empty);
            text = Regex.Replace(text, @"[\d-]", string.Empty);
            while (text.Contains("  ")) text = text.Replace("  ", " ");
            Logger.DebugFormat(Resources.TextCleanedMessage, text);
            return text;
        }

        public static HtmlDocument CleanupDocument(HtmlDocument document)
        {
            foreach (var script in document.DocumentNode.Descendants("script").ToArray())
                script.Remove();
            foreach (var style in document.DocumentNode.Descendants("style").ToArray())
                style.Remove();

            foreach (var comment in document.DocumentNode.SelectNodes("//comment()"))
            {
                comment.ParentNode.RemoveChild(comment);
            }

            return document;
        }
    }
}