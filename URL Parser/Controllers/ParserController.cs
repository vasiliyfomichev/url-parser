using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using URL_Parser.Contracts;
using URL_Parser.Filters;

namespace URL_Parser.Controllers
{
    public class ParserController : ApiController
    {
        #region Fields

        private readonly IUrlService _service;

        #endregion

        #region Constructor

        public ParserController(IUrlService service)
        {
            Debug.Assert(service!=null, "Bad service instance passed to controller.");
            _service = service;
        }

        #endregion

        [HttpGet, UrlValidator, UrlFormatter]
        public async Task<IEnumerable<Models.Image>> Images(string url)
        {
            var images = await _service.GetImagesAsync(url);
            return images;
        }

        [HttpGet, UrlValidator, UrlFormatter]
        public async Task<Dictionary<string,int>> WordReport(string url)
        {
            var words = await _service.GetWordReportAsync(url);
            return words;
        }
    }
}
