#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using URL_Parser.Models;

#endregion

namespace URL_Parser.Utility
{
    /// <summary>
    /// 
    /// </summary>
    public class UrlUtil
    {
        public static bool IsUri(string text)
        {
            Uri uriResult;
            var result = Uri.TryCreate(text, UriKind.Absolute, out uriResult);
            return result;
        }

        public static bool IsImageUrl(string url)
        {
            url = url.ToLower();
            var regex = new Regex(@"^.*\.(jpg|gif|jpeg|png|ico)$");
            var match = regex.Match(url);
            return match.Success;
        }

        /// <summary>
        /// Ensures the absolute URL for partial paths (i.e. relative file references).
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="requestUrl">The request URL.</param>
        /// <returns></returns>
        public static string EnsureAbsoluteUrl(string url, string requestUrl)
        {
            if ((IsAbsoluteUrl(url) && (requestUrl.ToLower().StartsWith("http") || requestUrl.ToLower().StartsWith("https"))) 
                || string.IsNullOrWhiteSpace(url)) return url;

            if (string.IsNullOrWhiteSpace(requestUrl)) return url;
            url = url.ToLower()
                .Replace("http://", string.Empty)
                .Replace("https://", string.Empty);

            var uri = new Uri(requestUrl);
            if (!url.StartsWith("..") && url.StartsWith("/"))
                return uri.Scheme + "://" + (uri.Host.EndsWith("/") ? uri.Host : uri.Host + "/") + url;

            var urlParts = uri.LocalPath.Split(new[]{'/'}, StringSplitOptions.RemoveEmptyEntries);
            var levelUpCount = 1;
            if (url.StartsWith(".."))
            {
                levelUpCount = new Regex(Regex.Escape("../")).Matches(url).Count;
                url = url.Replace("../", string.Empty);
            }
            url = string.Join("/", urlParts.Take(urlParts.Count() - levelUpCount)) + "/" + url;
            return uri.Scheme + "://" + (uri.Host.EndsWith("/") ? uri.Host : uri.Host + "/") + url;
        }

        public static string EnsureAbsoluteUrlFormat(string url, HttpContext context)
        {
            if (context == null || string.IsNullOrWhiteSpace(url)) return null;
            if (!url.ToLower().StartsWith("http") && !url.StartsWith("https"))
                url = string.Format("{0}://{1}", context.Request.Url.Scheme, url);
            return url;
        }

        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
        }

        public static bool UrlExists(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return false;
            url = EnsureAbsoluteUrlFormat(url, HttpContext.Current);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = WebRequestMethods.Http.Head;
            var response = (HttpWebResponse)request.GetResponse();
            return response.StatusCode == HttpStatusCode.OK;
        }

        public static string GetUrlContent(string url)
        {
            if (string.IsNullOrWhiteSpace(url) && !UrlExists(url))
                return null;
            var webRequest = WebRequest.Create(url);

            using (var response = webRequest.GetResponse())
            using (var content = response.GetResponseStream())
            {
                if (content == null) return null;
                using (var reader = new StreamReader(content))
                {
                    var strContent = reader.ReadToEnd();
                    return strContent;
                }
            }
        }

        public static IEnumerable<Image> GetMetaImageUrls(HtmlDocument document)
        {
            var head = document.DocumentNode.SelectSingleNode("//head");
            if (head == null) return null;
            var headNodes = head.ChildNodes;
            if (headNodes == null || !headNodes.Any()) return null;
            var urls = headNodes
                .Select(n =>
                    new Image
                    {
                        Src = n.GetAttributeValue("href", n.GetAttributeValue("content", null)),
                        Alt = n.GetAttributeValue("property", n.GetAttributeValue("name", "Meta image."))
                    }
                )
                .Where(i => !string.IsNullOrWhiteSpace(i.Src) && IsUri(i.Src) && IsImageUrl(i.Src));
            return urls;
        }
        
        public static IEnumerable<string> GetCssFilePaths(HtmlDocument document)
        {
            var rootNode = document.DocumentNode;
            if (rootNode == null) return null;

            var linkNodes = rootNode.SelectNodes("//link").ToList();
            if (!linkNodes.Any())
                return null;

            var nodes = linkNodes.Where(n =>n!=null &&
                !string.IsNullOrWhiteSpace(n.GetAttributeValue("rel", null)) && 
                (n.GetAttributeValue("rel", null).ToLower() == "stylesheet" || 
                (n.GetAttributeValue("type", null)!=null && n.GetAttributeValue("type", null).ToLower() == "text/css")))
                .Select(n=>n.GetAttributeValue("href", null));
            return nodes.Count(n=>n!=null)==0 ? null : nodes.Where(n => n != null);
        }

        public static IEnumerable<string> GetScriptFilePaths(HtmlDocument document)
        {
            var rootNode = document.DocumentNode;
            if (rootNode == null) return null;

            var scriptNodes = rootNode.SelectNodes("//script").ToList();
            if (!scriptNodes.Any())
                return null;

            var nodes = scriptNodes
                .Select(n => n.GetAttributeValue("src", null));
            return nodes.Count(n => n != null) == 0 ? null : nodes.Where(n => n != null);
        }
    }
}