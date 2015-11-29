#region

using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using URL_Parser.Models;

#endregion

namespace URL_Parser.Contracts
{
    public interface IUrlService
    {
        Task<IEnumerable<WordReportItem>> GetWordReportAsync(string url, int maxReportSize);
        Task<IEnumerable<Image>> GetImagesAsync(string url, HttpContext context);
    }
}