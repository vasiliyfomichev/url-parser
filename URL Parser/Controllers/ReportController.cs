#region

using System.Web.Mvc;

#endregion

namespace URL_Parser.Controllers
{
    public class ReportController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
    }
}