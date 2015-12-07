#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;
using log4net;
using Microsoft.Ajax.Utilities;
using URL_Parser.Configuration;
using URL_Parser.Contracts;
using URL_Parser.Models;
using URL_Parser.Properties;
using URL_Parser.Utility;

#endregion

namespace URL_Parser.Providers
{
    public class UrlService : IUrlService
    {
        #region Fields

        private static ILog _logger;

        #endregion

        #region Constructor

        public UrlService(ILog logger)
        {
            _logger = logger;
        }

        #endregion

        #region IUrlService Members

        public IEnumerable<WordReportItem> GetWordReport(string url, int maxReportSize)
        {
            var words = GetWords(url, maxReportSize);
            return words;
        }

        public IEnumerable<Image> GetImages(string url, HttpContext context)
        {
            if (context == null) return null;
            var images = GetAllImageReferences(url, context);
            return images;
        }

        #endregion

        #region Private Methods

        private static IEnumerable<Image> GetAllImageReferences(string url, HttpContext context)
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
            imageUrls = imageUrls.DistinctBy(i=>i.Src.ToLower()).ToList();
            _logger.DebugFormat(Resources.GetGeneratedImagePathsMessage, 
                imageUrls.Count, 
                url,
                imageUrls.Any() ? string.Join(", ", imageUrls.Select(i => i.Src)) : "none");
            return imageUrls;
        }

        private static IEnumerable<WordReportItem> GetWords(string url, int maxReportSize)
        {
            var document = new HtmlWeb().Load(url);
            document = HtmlUtil.CleanupDocument(document);
            if (document.DocumentNode.SelectSingleNode("//body") == null) throw new GenericException();
            var htmlContentElements = document.DocumentNode.SelectSingleNode("//body")
                .DescendantsAndSelf()
                .ToList();

            var text = HtmlUtil.GetContentOfHtmlTextNodes(htmlContentElements);


            var cleanText = HtmlUtil.CleanupString(text);
            _logger.DebugFormat(Resources.ContentRetrievedMessage, cleanText);
            
            var specialCharacters = Settings.Default.SpecialCharacters;
            specialCharacters.Add(" "); // Adding mandatory space to split by
            var charArrayToSplitBy = string.Join(string.Empty, specialCharacters.Cast<string>().ToArray()).ToCharArray();
            if (string.IsNullOrWhiteSpace(cleanText)) return null;
            var wordArray = cleanText.Split(charArrayToSplitBy, StringSplitOptions.RemoveEmptyEntries);
            var rankings = wordArray.GroupBy(i => i.ToLower()).Select(g =>
                new WordReportItem {Word = g.Key, Count = g.Count()}
                );

            rankings = rankings.OrderByDescending(w => w.Count).ThenBy(w => w.Word);

            var cleanRankings = rankings.Where(ranking => !Settings.Default.StopWords.Contains(ranking.Word)).ToList();

            if (cleanRankings.Count() > maxReportSize)
                cleanRankings = cleanRankings.Take(maxReportSize).ToList();

            _logger.DebugFormat(Resources.GeneratedWordReportMessage, 
                (cleanRankings.Any() ? 
                string.Join(", ",cleanRankings.Select(i => i.Word + ":" + i.Count)) : "none"));

            return cleanRankings;
        }

        #endregion
    }
}



