using System.Collections.Generic;
using System.Threading.Tasks;
using URL_Parser.Models;

namespace URL_Parser.Contracts
{
    public interface IUrlService
    {
        Task<Dictionary<string, int>> GetWordReportAsync(string url);
        Task<IEnumerable<Image>> GetImagesAsync(string url);
    }
}