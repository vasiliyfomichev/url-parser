#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public Task<IEnumerable<WordReportItem>> GetWordReportAsync(string url, int maxReportSize)
        {
            var task = Task.Run(() => ParseWords(url, maxReportSize));
            return task;
        }

        public Task<IEnumerable<Image>> GetImagesAsync(string url, HttpContext context)
        {
            if (context == null) return null;
            var task = Task.Run(() => ParseImages(url, context));
            return task;
        }

        #endregion

        #region Private Methods

        private static IEnumerable<Image> ParseImages(string url, HttpContext context)
        {
                var document = new HtmlWeb().Load(url);
                var imageUrls = document.DocumentNode.Descendants("img")
                    .Select(e =>
                        new Image
                        {
                            Src = UrlUtil.EnsureAbsoluteUrl(e.GetAttributeValue("src", null), url),
                            Alt = e.GetAttributeValue("alt", null)
                        })
                    .Where(s => !string.IsNullOrEmpty(s.Src))
                    .ToList();

                var urlsFromHead = UrlUtil.GetMetaImageUrls(document);
                imageUrls.AddRange(urlsFromHead);

            // parsing css
            // Referenced CCS
            var cssPaths = UrlUtil.GetCssFilePaths(document);
            foreach (var path in cssPaths)
            {
                var imageReferences = ImageUtil.GetImagesFromCssFile(path, context);
                if (imageReferences==null || !imageReferences.Any()) 
                    continue;
                imageUrls.AddRange(imageReferences.Select(i=>new Image{
                    Src = UrlUtil.EnsureAbsoluteUrl(i.Src, path),
                    Alt = i.Alt
                    }));
            }

            // Inline CCS
            var inlineStyles = document.DocumentNode.SelectNodes("//style");
            foreach (var inlineStyle in inlineStyles)
            {
                var styleContent = inlineStyle.InnerText;
                var regex = Settings.Default.ImageRegexPatternForCss;
                var imageReferences = ImageUtil.GetImagesFromText(styleContent, regex);
                if (imageReferences == null || !imageReferences.Any())
                    continue;
                imageUrls.AddRange(imageReferences.Select(i => new Image
                {
                    Src = UrlUtil.EnsureAbsoluteUrl(i.Src, url),
                    Alt = i.Alt
                }));
            }

            // Referenced JS
            var scriptPaths = UrlUtil.GetScriptFilePaths(document);
            foreach (var path in scriptPaths)
            {
                var imageReferences = ImageUtil.GetImagesFromScriptFile(path, context);
                if (imageReferences == null || !imageReferences.Any())
                    continue;
                imageUrls.AddRange(imageReferences.Select(i => new Image
                {
                    Src = UrlUtil.EnsureAbsoluteUrl(i.Src, path),
                    Alt = i.Alt
                }));
            }

            // Inline JS
            var inlineScripts = document.DocumentNode.SelectNodes("//script");
            foreach (var inlineScript in inlineScripts)
            {
                var styleContent = inlineScript.InnerText;
                var regex = Settings.Default.ImageRegexPatternForJs;
                var imageReferences = ImageUtil.GetImagesFromText(styleContent, regex);
                if (imageReferences == null || !imageReferences.Any())
                    continue;
                imageUrls.AddRange(imageReferences.Select(i => new Image
                {
                    Src = UrlUtil.EnsureAbsoluteUrl(i.Src, url),
                    Alt = i.Alt
                }));
            }
            
            return imageUrls;
        }

        private static IEnumerable<WordReportItem> ParseWords(string url, int maxReportSize)
        {
            var document = new HtmlWeb().Load(url);

            foreach (var script in document.DocumentNode.Descendants("script").ToArray())
                script.Remove();
            foreach (var style in document.DocumentNode.Descendants("style").ToArray())
                style.Remove();

            foreach (var comment in document.DocumentNode.SelectNodes("//comment()"))
            {
                comment.ParentNode.RemoveChild(comment);
            }
            var htmlElements = document.DocumentNode.SelectSingleNode("//body")
                .DescendantsAndSelf()
                .ToList();

            var words = new StringBuilder();

            var htmlTagsWithText =
                htmlElements.OfType<HtmlTextNode>().Where(text => !string.IsNullOrWhiteSpace(text.Text));
            Parallel.ForEach(htmlTagsWithText, (htmlTagWithText) => words.Append(StripNewLines(htmlTagWithText.Text) + " "));

            var wordString = words.ToString();
            var source = wordString.Split(new[] { '.', '?', '!', ' ', ';', ':', ',' },
                StringSplitOptions.RemoveEmptyEntries);
            var rankings = source.GroupBy(i => i).Select(g =>
                new WordReportItem { Word = g.Key, Count = g.Count() }
            );

            rankings = rankings.OrderByDescending(w => w.Count);
            var cleanRankings = rankings.Where(ranking => !Settings.Default.StopWords.Contains(ranking.Word)).ToList();

            if (cleanRankings.Count() > maxReportSize)
                cleanRankings = cleanRankings.Take(maxReportSize).ToList();

            return cleanRankings;
        }

        private static string StripNewLines(string text)
        {
            return string.IsNullOrEmpty(text) ? null : Regex.Replace(text, @"\t|\n|\r", string.Empty).Trim();
        }
        
        #endregion

    }
}



