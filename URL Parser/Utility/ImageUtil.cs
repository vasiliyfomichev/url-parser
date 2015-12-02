#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using URL_Parser.Models;
using URL_Parser.Properties;

#endregion

namespace URL_Parser.Utility
{
    public class ImageUtil
    {

        public static IEnumerable<Image> GetImagesFromImageTags(HtmlDocument document, string url)
        {
            var imageUrls = document.DocumentNode.Descendants("img")
                    .Select(e =>
                        new Image
                        {
                            Src = UrlUtil.EnsureAbsoluteUrl(e.GetAttributeValue("src", null), url),
                            Alt = e.GetAttributeValue("alt", null)
                        })
                    .Where(s => !string.IsNullOrEmpty(s.Src))
                    .ToList();
            return imageUrls;
        }

        public static IEnumerable<Image> GetImagesFromReferencedCss(HtmlDocument document, string url,
            HttpContext context)
        {
            var cssPaths = UrlUtil.GetCssFilePaths(document);
            var imageUrls = new List<Image>();
            if (cssPaths == null || !cssPaths.Any()) return imageUrls;
            foreach (var path in cssPaths)
            {
                var imageReferences = ImageUtil.GetImagesFromCssFile(path, context);
                if (imageReferences == null || !imageReferences.Any())
                    continue;
                imageUrls.AddRange(imageReferences.Select(i => new Image
                {
                    Src = UrlUtil.EnsureAbsoluteUrl(i.Src, path),
                    Alt = i.Alt
                }));
            }
            return imageUrls;
        }

        public static IEnumerable<Image> GetImagesFromInlineCss(HtmlDocument document, string url)
        {
            var imageUrls = new List<Image>();
            var inlineStyles = document.DocumentNode.SelectNodes("//style");
            if (inlineStyles == null || !inlineStyles.Any()) return imageUrls;

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
            return imageUrls;
        }


        public static IEnumerable<Image> GetImagesFromReferencedJs(HtmlDocument document, string url, HttpContext context)
        {
            var imageUrls = new List<Image>();
            var scriptPaths = UrlUtil.GetScriptFilePaths(document);
            if (scriptPaths == null || !scriptPaths.Any()) return imageUrls;

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

            return imageUrls;
        }

        public static IEnumerable<Image> GetImagesFromInlineJs(HtmlDocument document, string url)
        {
            var imageUrls = new List<Image>();
            var inlineScripts = document.DocumentNode.SelectNodes("//script");
            if (inlineScripts == null || !inlineScripts.Any()) return imageUrls;

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

        public static IEnumerable<Image> GetImagesFromCssFile(string filePath, HttpContext context)
        {
            if (String.IsNullOrWhiteSpace(filePath) || context == null) return null;

            filePath = UrlUtil.EnsureAbsoluteUrlFormat(filePath, context);

            var regex = Settings.Default.ImageRegexPatternForCss;
            const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            var content = UrlUtil.GetUrlContent(filePath);
            if (String.IsNullOrWhiteSpace(content)) return null;

            var matches = Regex.Matches(content, regex, options)
                .Cast<Match>().Select(m => m.Groups["bgpath"]);
            return matches.Select(m => m.Value)
                .Where(v => v.EndsWith("jpg", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("gif", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("png", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("ico", StringComparison.OrdinalIgnoreCase))
                .Select(r => new Image
                {
                    Src = r,
                    Alt = Settings.Default.DefaultAltForImageFromCssFiles
                });
        }

        public static IEnumerable<Image> GetImagesFromScriptFile(string filePath, HttpContext context)
        {

            if (String.IsNullOrWhiteSpace(filePath) || context == null) return null;

            filePath = UrlUtil.EnsureAbsoluteUrlFormat(filePath, context);

            var regex = Settings.Default.ImageRegexPatternForJs;
            const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            var content = UrlUtil.GetUrlContent(filePath);
            if (String.IsNullOrWhiteSpace(content)) return null;

            var matches = Regex.Matches(content, regex, options)
                .Cast<Match>().Select(m => m.Groups["bgpath"]);
            return matches.Select(m => m.Value)
                .Where(v => v.EndsWith("jpg", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("gif", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("png", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("ico", StringComparison.OrdinalIgnoreCase))

                //TODO:account for images with querystrings
                .Select(r => new Image
                {
                    Src = r,
                    Alt = Settings.Default.DefaultAltForImageFromJsFiles
                });
        }

        public static IEnumerable<Image> GetImagesFromText(string text, string imageRegex)
        {
            if (String.IsNullOrWhiteSpace(text)) return null;
            const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            if (String.IsNullOrWhiteSpace(text)) return null;

            var matches = Regex.Matches(text, imageRegex, options)
                .Cast<Match>().Select(m => m.Groups["bgpath"]);
            return matches.Select(m => m.Value)
                .Where(v => v.EndsWith("jpg", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("gif", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("jpeg", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("png", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith("ico", StringComparison.OrdinalIgnoreCase))
                .Select(r => new Image
                {
                    Src = r,
                    Alt = Settings.Default.DefaultAltForImageFromInlineCode
                });
        }
    }
}