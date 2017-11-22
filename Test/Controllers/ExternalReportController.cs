namespace Fanex.AccountingReports.Test.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Test.Services.Implement;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExternalReportController : BaseController
    {
        private readonly ExternalDailyWinlostService _reportService;

        public ExternalReportController()
        {
            _reportService = new ExternalDailyWinlostService();
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View("~/Areas/Test/Views/Grid/ExternalReport.cshtml");
        }

        [HttpGet]
        public ActionResult IndexRazor()
        {
            var result = _reportService.GetDailyWinLoss();
            return View(result);
        }

        [HttpPost]
        public JsonResult GetDailyWinlost()
        {
            var result = _reportService.GetDailyWinLoss();
            return Json(result);
        }
    }
}
