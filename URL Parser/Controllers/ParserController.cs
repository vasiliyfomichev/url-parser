#region

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using log4net;
using log4net.Core;
using URL_Parser.Contracts;
using URL_Parser.Filters;
using URL_Parser.Models;
using System;

#endregion

namespace URL_Parser.Controllers
{
    public class ParserController : ApiController
    {
        #region Fields

        private readonly IUrlService _service;
        private readonly ILog _logger;

        #endregion

        #region Constructor

        public ParserController(IUrlService service, ILog logger)
        {
            _logger = logger;
            _service = service;
        }

        #endregion

        [HttpGet, UrlValidator, UrlFormatter]
        public async Task<IEnumerable<Image>> Images(string url)
        {
            try
            {
                var images = await _service.GetImagesAsync(url, HttpContext.Current);
                return images;
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
                throw ex;
            }
        }

        [HttpGet, UrlValidator, UrlFormatter]
        public async Task<IEnumerable<WordReportItem>> WordReport(string url, int maxReportSize = 20)
        {
            var words = await _service.GetWordReportAsync(url, maxReportSize);
            return words;
        }
    }
}
