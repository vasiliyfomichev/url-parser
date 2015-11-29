using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using URL_Parser.Contracts;
using URL_Parser.Models;

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

        public Task<IEnumerable<Image>> GetImagesAsync(string url)
        {
            var task = Task.Run(() => ParseImages(url));
            return task;
        }

        #endregion

        #region Private Methods

        private static IEnumerable<Image> ParseImages(string url)
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



