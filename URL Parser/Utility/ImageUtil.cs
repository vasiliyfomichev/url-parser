#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using URL_Parser.Models;
using URL_Parser.Properties;

#endregion

namespace URL_Parser.Utility
{
    public class ImageUtil
    {
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
                    Alt = "Styling image"
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
                    Alt = "Script image"
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
                    Alt = "Inline image"
                });
        }
    }
}