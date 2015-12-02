#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using URL_Parser.Contracts;
using URL_Parser.Models;
using URL_Parser.Properties;
using URL_Parser.Utility;

#endregion

namespace URL_Parser.Providers
{
    public class UrlService : IUrlService
    {
        #region IUrlService Members

        public IEnumerable<WordReportItem> GetWordReport(string url, int maxReportSize)
        {
            var words = ParseWords(url, maxReportSize);
            return words;
        }

        public IEnumerable<Image> GetImages(string url, HttpContext context)
        {
            if (context == null) return null;
            var images = ParseImages(url, context);
            return images;
        }

        #endregion

        #region Private Methods

        private static IEnumerable<Image> ParseImages(string url, HttpContext context)
        {
            var document = new HtmlWeb().Load(url);
            var imageUrls = new List<Image>();
            Task.WaitAll(new[]
            {
                Task.Run(() => imageUrls.AddRange(UrlUtil.GetMetaImageUrls(document))),
                Task.Run(() => imageUrls.AddRange(ImageUtil.GetImagesFromImageTags(document, url))),
                Task.Run(() => imageUrls.AddRange(ImageUtil.GetImagesFromReferencedCss(document, url, context))),
                Task.Run(() => imageUrls.AddRange(ImageUtil.GetImagesFromInlineCss(document, url))),
                Task.Run(() => imageUrls.AddRange(ImageUtil.GetImagesFromReferencedJs(document, url, context))),
                Task.Run(() => imageUrls.AddRange(ImageUtil.GetImagesFromInlineJs(document, url)))
            });

            return imageUrls;
        }

        private static IEnumerable<WordReportItem> ParseWords(string url, int maxReportSize)
        {
            var document = new HtmlWeb().Load(url);
            document = CleanupDocument(document);

            var htmlContentElements = document.DocumentNode.SelectSingleNode("//body")
                .DescendantsAndSelf()
                .ToList();

            var htmlTagsWithText = string.Join(" ", htmlContentElements
                .Where(n => !n.HasChildNodes && !string.IsNullOrWhiteSpace(n.InnerText))
                .Select(n => n.InnerText).ToList());


            var wordString = CleanupString(htmlTagsWithText);
            var specialCharacters = Settings.Default.SpecialCharacters;
            specialCharacters.Add(" "); // Adding mandatory space to split by
            var charArrayToSplitBy = string.Join(string.Empty, specialCharacters.Cast<string>().ToArray()).ToCharArray();
            var wordArray = wordString.Split(charArrayToSplitBy, StringSplitOptions.RemoveEmptyEntries);
            var rankings = wordArray.GroupBy(i => i.ToLower()).Select(g =>
                new WordReportItem {Word = g.Key, Count = g.Count()}
                );

            rankings = rankings.OrderByDescending(w => w.Count).ThenBy(w => w.Word);
            var cleanRankings = rankings.Where(ranking => !Settings.Default.StopWords.Contains(ranking.Word)).ToList();

            if (cleanRankings.Count() > maxReportSize)
                cleanRankings = cleanRankings.Take(maxReportSize).ToList();

            return cleanRankings;
        }

        #region Helpers

        private static string CleanupString(string text)
        {
            if (string.IsNullOrEmpty(text)) return null;
            text = Regex.Replace(text, Settings.Default.NewLineRegex, string.Empty).Trim();
            text = HttpUtility.HtmlDecode(text);
            text = Regex.Replace(text, @"[^\u0000-\u007F]", string.Empty);
            text = Regex.Replace(text, @"[\d-]", string.Empty);
            return text;
        }

        private static HtmlDocument CleanupDocument(HtmlDocument document)
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

        #endregion

        #endregion
    }
}



