using System.Security.Policy;
using System.Web;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;
using URL_Parser.Contracts;
using URL_Parser.Models;
using URL_Parser.Properties;

namespace URL_Parser.Providers
{
    public class UrlService : IUrlService
    {
        #region IUrlService Members

        public Task<Dictionary<string, int>> GetWordReportAsync(string url)
        {
            var task = Task.Run(() => ParseWords(url));
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
                var imageReferences = UrlUtil.GetImagesFromCssFile(path, context);
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
                var imageReferences = UrlUtil.GetImagesFromText(styleContent, regex);
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
                var imageReferences = UrlUtil.GetImagesFromScriptFile(path, context);
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
                var imageReferences = UrlUtil.GetImagesFromText(styleContent, regex);
                if (imageReferences == null || !imageReferences.Any())
                    continue;
                imageUrls.AddRange(imageReferences.Select(i => new Image
                {
                    Src = UrlUtil.EnsureAbsoluteUrl(i.Src, url),
                    Alt = i.Alt
                }));
            }

                //TODO:parse css and js references.
                return imageUrls;
        }

        private static Dictionary<string, int> ParseWords(string url)
        {
            var document = new HtmlWeb().Load(url);
            document.DocumentNode.SelectSingleNode("//body")
                .DescendantsAndSelf()
                .Where(n=>n.Name.ToLower()=="script" || n.Name.ToLower()=="style" || n.NodeType!= HtmlNodeType.Comment)
                .ToList()
                .ForEach(n=>n.Remove());
            var root = document.DocumentNode.SelectSingleNode("//body");
            if (root == null) return null; //TODO: weired shit with scripts and css
            var words = new StringBuilder();

            words.Append(root.InnerText);
            words.Append(string.Join(" ", root.DescendantsAndSelf()
                .Where(d => !string.IsNullOrWhiteSpace(d.InnerText))
                .Select(d => d.InnerText)));
            var wordString = Regex.Replace(words.ToString(), @"\t|\n|\r", string.Empty);

            var source = wordString.Split(new[] { '.', '?', '!', ' ', ';', ':', ',' },
                StringSplitOptions.RemoveEmptyEntries);

            var rankings = source.GroupBy(i => i).Select(g =>
                new { Word = g.Key, Count = g.Count() }
            );

            return rankings.ToDictionary(wc=>wc.Word, wc=>wc.Count);
        }

        #endregion

    }
}



