namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria.NegativeBalance;
    using Fanex.AccountingReports.Licensee.Models;
    using Fanex.AccountingReports.Licensee.Models.NegativeBalance;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    public class NegativeBalanceController : BaseController
    {
        private readonly NegativeBalanceService _reportService;
        private readonly LicenseeSiteService _licenseeSite;

        public NegativeBalanceController()
        {
            _reportService = new NegativeBalanceService();
            _licenseeSite = new LicenseeSiteService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NegativeBalance, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            var licenseeSiteList = _licenseeSite.GetLicenseeSiteList();
            var all = new Licensee { LicenseeName = "All", LicenseeId = -1 };
            licenseeSiteList.Insert(0, all);
            ViewBag.LicensseSiteList = new SelectList(licenseeSiteList, "LicenseeName", "LicenseeName");
            return View();
        }

        [HttpPost]
        public JsonResult GetNegativeBalance(NegativeBalanceCriteria negativeBalanceCriteria)
        {
            var result = _reportService.GetNegativeBalance(negativeBalanceCriteria);
            return Json(result);
        }

        [HttpPost]
        public JsonResult SendMail(MailParam mail, NegativeBalanceCriteria negativeBalanceCriteria)
        {
            var result = _reportService.SendMail(mail, negativeBalanceCriteria);
            return Json(result);
        }

        [HttpGet]
        public ActionResult ExportNegativeBalanceReport(NegativeBalanceCriteria negativeBalanceCriteria)
        {
            _reportService.ExportExcelNegativeBalance(negativeBalanceCriteria);
            return new EmptyResult();
        }
    }
}