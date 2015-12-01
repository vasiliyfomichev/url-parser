#region

using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using log4net;
using log4net.Core;
using URL_Parser.Configuration.Filters;
using URL_Parser.Contracts;
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

        [HttpGet]
        public async Task<IHttpActionResult> Images(string url)
        {
            _logger.Debug(string.Format("Received image parsing request for {0}.", url));
            var images = await _service.GetImagesAsync(url, HttpContext.Current);
            return Ok(images);
        }

        [HttpGet]
        public async Task<IHttpActionResult> WordReport(string url, int maxReportSize = 20)
        {
            _logger.Debug(string.Format("Received word parsing request for {0} URL with a {1} limit.", url, maxReportSize));
            var words = await _service.GetWordReportAsync(url, maxReportSize);
            return Ok(words);
        }
    }
}
