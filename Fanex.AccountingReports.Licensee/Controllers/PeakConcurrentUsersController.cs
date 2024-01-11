namespace Fanex.AccountingReports.Licensee.Controllers
{
    using AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria.PeakConcurrentUser;
    using Fanex.AccountingReports.Licensee.Models.PeakConcurrentUser;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System.Web.Mvc;

    public class PeakConcurrentUsersController : BaseController
    {
        private readonly PeakConcurrentUserService _reportService;

        public PeakConcurrentUsersController()
        {
            _reportService = new PeakConcurrentUserService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.PeakConcurrentUsers, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            ViewBag.LicensseSiteList = new SelectList(_reportService.GetLicensee(), "LicenseeName", "LicenseeName");
            return View();
        }

        [HttpPost]
        public JsonResult Partial(PeakConcurrentUserCriteria peakConcurrentUserCriteria)
        {
            var result = new PeakConcurrentUser();
            result = _reportService.GetConcurrentUserList(peakConcurrentUserCriteria);
            return Json(result);
        }

        [HttpGet]
        public ActionResult ExportGamePeakReport(PeakConcurrentUserCriteria peakConcurrentUserCriteria)
        {
            _reportService.PeakExport2Excel(peakConcurrentUserCriteria);
            return new EmptyResult();
        }
    }
}
