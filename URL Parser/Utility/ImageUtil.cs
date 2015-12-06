#region

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using log4net;
using URL_Parser.Models;
using URL_Parser.Properties;

#endregion

namespace URL_Parser.Utility
{
    public class ImageUtil
    {
        #region Fields

        private static readonly ILog Logger = LogManager.GetLogger(typeof(MvcApplication));

        #endregion

        #region Methods

        public static IEnumerable<Image> GetImagesFromImageTags(HtmlDocument document, string url)
        {
            var images = document.DocumentNode.Descendants("img")
                    .Select(e =>
                        new Image
                        {
                            Src = UrlUtil.EnsureAbsoluteUrl(e.GetAttributeValue("src", null), url),
                            Alt = e.GetAttributeValue("alt", null)
                        })
                    .Where(s => !string.IsNullOrEmpty(s.Src))
                    .ToList();
            Logger.DebugFormat(Resources.ImagesReturnedMethodMessage, images != null ? images.Count() : 0, GetCurrentMethod());
            return images;
        }

        public static IEnumerable<Image> GetImagesFromReferencedCss(HtmlDocument document, string url,
            HttpContext context)
        {
            var cssPaths = UrlUtil.GetCssFilePaths(document);
            var images = new List<Image>();
            if (cssPaths == null || !cssPaths.Any()) return images;
            foreach (var path in cssPaths)
            {
                var imageReferences = GetImagesFromCssFile(path, context);
                if (imageReferences == null || !imageReferences.Any())
                    continue;
                images.AddRange(imageReferences.Select(i => new Image
                {
                    Src = UrlUtil.EnsureAbsoluteUrl(i.Src, path),
                    Alt = i.Alt
                }));
            }
            Logger.DebugFormat(Resources.ImagesReturnedMethodMessage, images != null ? images.Count() : 0, GetCurrentMethod());
            return images;
        }

        public static IEnumerable<Image> GetImagesFromInlineCss(HtmlDocument document, string url)
        {
            var documentRoot = document.DocumentNode;
            var documentText = documentRoot.InnerHtml;

            var images = new List<Image>();
            var regex = Settings.Default.ImageRegexPatternForCss;
            var imageReferences = GetImagesFromText(documentText, regex);
            if (imageReferences == null || !imageReferences.Any())
                return images;
            images.AddRange(imageReferences.Select(i => new Image
            {
                Src = UrlUtil.EnsureAbsoluteUrl(i.Src, url),
                Alt = i.Alt
            }));
            Logger.DebugFormat(Resources.ImagesReturnedMethodMessage, images != null ? images.Count() : 0,
                GetCurrentMethod());
            return images;
        }

        public static IEnumerable<Image> GetImagesFromReferencedJs(HtmlDocument document, string url, HttpContext context)
        {
            var images = new List<Image>();
            var scriptPaths = UrlUtil.GetScriptFilePaths(document);
            if (scriptPaths == null || !scriptPaths.Any()) return images;

            Parallel.ForEach(scriptPaths, (scriptPath) =>
            {
                var imageReferences = GetImagesFromScriptFile(scriptPath, context);
                if (imageReferences == null || !imageReferences.Any()) return;
                images.AddRange(imageReferences.Select(i => new Image
                {
                    Src = UrlUtil.EnsureAbsoluteUrl(i.Src, scriptPath),
                    Alt = i.Alt
                }));
            });

            Logger.DebugFormat(Resources.ImagesReturnedMethodMessage, images != null ? images.Count() : 0, GetCurrentMethod());
            return images;
        }

        public static IEnumerable<Image> GetImagesFromInlineJs(HtmlDocument document, string url)
        {
            var images = new List<Image>();
            var inlineScripts = document.DocumentNode.SelectNodes("//script");
            if (inlineScripts == null || !inlineScripts.Any()) return images;
            Parallel.ForEach(inlineScripts, (inlineScript) =>
            {
                var styleContent = inlineScript.InnerText;
                var regex = Settings.Default.ImageRegexPatternForJs;
                var imageReferences = GetImagesFromText(styleContent, regex);
                if (imageReferences == null || !imageReferences.Any())
                    return;
                images.AddRange(imageReferences.Select(i => new Image
                {
                    Src = UrlUtil.EnsureAbsoluteUrl(i.Src, url),
                    Alt = i.Alt
                }));
            });

            Logger.DebugFormat(Resources.ImagesReturnedMethodMessage, images != null ? images.Count() : 0, GetCurrentMethod());
            return images;
        }

        public static IEnumerable<Image> GetImagesFromCssFile(string filePath, HttpContext context)
        {
            if (String.IsNullOrWhiteSpace(filePath) || context == null) return null;

            filePath = UrlUtil.EnsureAbsoluteUrlFormat(filePath, context);

            var regex = Settings.Default.ImageRegexPatternForCss;
            const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            var content = UrlUtil.GetUrlContent(filePath);
            if (string.IsNullOrWhiteSpace(content)) return null;

            var matches = Regex.Matches(content, regex, options)
                .Cast<Match>().Select(m => m.Groups["url"]);
            var images =  matches.Select(m => m.Value)
                .Select(r => new Image
                {
                    Src = r,
                    Alt = Settings.Default.DefaultAltForImageFromCssFiles
                });
            Logger.DebugFormat(Resources.ImagesReturnedMethodMessage, images != null ? images.Count() : 0, GetCurrentMethod());
            return images;
        }

        public static IEnumerable<Image> GetImagesFromScriptFile(string filePath, HttpContext context)
        {

            if (String.IsNullOrWhiteSpace(filePath) || context == null) return null;

            filePath = UrlUtil.EnsureAbsoluteUrlFormat(filePath, context);

            var regex = Settings.Default.ImageRegexPatternForJs;
            const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            var content = UrlUtil.GetUrlContent(filePath);
            if (string.IsNullOrWhiteSpace(content)) return null;

            var matches = Regex.Matches(content, regex, options)
                .Cast<Match>().Select(m => m.Groups["url"]);
            var images =  matches.Select(m => m.Value)
                .Where(v => v.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))

                //TODO:account for images with querystrings
                .Select(r => new Image
                {
                    Src = r,
                    Alt = Settings.Default.DefaultAltForImageFromJsFiles
                });

            Logger.DebugFormat(Resources.ImagesReturnedMethodMessage, images != null ? images.Count() : 0, GetCurrentMethod());
            return images;
        }

        public static IEnumerable<Image> GetImagesFromText(string text, string imageRegex)
        {
            if (String.IsNullOrWhiteSpace(text)) return null;
            const RegexOptions options = ((RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline) | RegexOptions.IgnoreCase);
            if (String.IsNullOrWhiteSpace(text)) return null;

            var matches = Regex.Matches(text, imageRegex, options)
                .Cast<Match>().Select(m => m.Groups["url"]);
            var images = matches.Select(m => m.Value)
                .Where(v => v.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                            || v.EndsWith(".ico", StringComparison.OrdinalIgnoreCase))
                            .Where(v=>!v.Trim().StartsWith("."))
                .Select(r => new Image
                {
                    Src = r,
                    Alt = Settings.Default.DefaultAltForImageFromInlineCode
                });
            return images;
        }

        #endregion

        #region Helpers

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            var stack = new StackTrace();
            var frame = stack.GetFrame(1);

            return frame.GetMethod().Name;
        }

        #endregion
    }
}