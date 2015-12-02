#region

using System.Collections.Generic;
using System.Web;
using URL_Parser.Models;

#endregion

namespace URL_Parser.Contracts
{
    public interface IUrlService
    {
        IEnumerable<WordReportItem> GetWordReport(string url, int maxReportSize);
        IEnumerable<Image> GetImages(string url, HttpContext context);
    }
}