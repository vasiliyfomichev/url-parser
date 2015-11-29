using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using HtmlAgilityPack;
using URL_Parser.Models;

namespace URL_Parser
{
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

        public static string EnsureAbsoluteUrl(string url, string requestUrl)
        {
            if (IsAbsoluteUrl(url) || string.IsNullOrWhiteSpace(url)) return url;

            if (string.IsNullOrWhiteSpace(requestUrl)) return url;
            var uri = new Uri(requestUrl);
            return uri.Scheme + "://" + uri.Host + url; //TODO: validate the forward slash presence and //
        }

        public static bool IsAbsoluteUrl(string url)
        {
            Uri result;
            return Uri.TryCreate(url, UriKind.Absolute, out result);
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

        public static string EnsureAbsoluteUrlFormat(string url, HttpContext context)
        {
            if (context == null || string.IsNullOrWhiteSpace(url)) return null;
            if (!url.ToLower().StartsWith("http") && !url.StartsWith("https"))
                url = string.Format("{0}://{1}", context.Request.Url.Scheme, url);
            return url;
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
    }
}