namespace Fanex.AccountingReports.Balance.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Balance.Services.Implement;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CompanyBalanceDifferenceController : BaseController
    {
        private readonly CompanyBalanceDifferenceService _reportService;

        public CompanyBalanceDifferenceController()
        {
            this._reportService = new CompanyBalanceDifferenceService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.CompanyBalanceDifference, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetCompanyBalanceDifference()
        {
            var report = this._reportService.GetCompanyBalanceDifference();
            var jsonResult = Json(report);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }
    }
}