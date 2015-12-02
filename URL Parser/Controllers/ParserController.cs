﻿#region

using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using log4net;
using URL_Parser.Contracts;
using URL_Parser.Properties;

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

        [System.Web.Http.HttpGet, OutputCache(CacheProfile = "CacheClient1hr")]
        public IHttpActionResult Images(string url)
        {
            _logger.Debug(string.Format(Resources.ReceivedParcingRequestMessageForImages, url));
            var images = _service.GetImages(url, HttpContext.Current);
            return Ok(images);
        }

        [System.Web.Http.HttpGet, OutputCache(CacheProfile = "CacheClient1hr")]
        public IHttpActionResult WordReport(string url, int maxReportSize = 20)
        {
            _logger.Debug(string.Format(Resources.ReceivedParcingRequestMessageForWords, url, maxReportSize));
            var words = _service.GetWordReport(url, maxReportSize);
            return Ok(words);
        }
    }
}
