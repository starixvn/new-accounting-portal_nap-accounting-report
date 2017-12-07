namespace Fanex.AccountingReports.Balance.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Balance.Criteria.DirectBalance;
    using Fanex.AccountingReports.Balance.Services.Implement;
    using Fanex.AccountingReports.Balance.Services.Interface;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Core.Web.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DirectBalanceController : BaseController
    {
        private readonly IDirectBalanceService _directBalanceService;

        public DirectBalanceController()
        {
            this._directBalanceService = new DirectBalanceService();
        }

        [HttpGet]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "Index")]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDirectBalance(GetDirectBalanceCriteria getDirectBalanceCriteria)
        {
            var result = this._directBalanceService.GetDirectBalance(getDirectBalanceCriteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetDirectBalanceMasterDownline(GetDirectBalanceMasterDownlineCriteria getDirectBalanceMasterDownlineCriteria)
        {
            var result = this._directBalanceService.GetDirectBalanceMasterDownLine(getDirectBalanceMasterDownlineCriteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public JsonResult GetDirectBalanceAgentDownline(GetDirectBalanceAgentDownlineCriteria getDirectBalanceAgentDownlineCriteria)
        {
            var result = this._directBalanceService.GetDirectBalanceAgentDownLine(getDirectBalanceAgentDownlineCriteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public ActionResult ExportReport(GetDirectBalanceCriteria getDirectBalanceCriteria)
        {
            var result = this._directBalanceService.ExportDirectBalanceReport(getDirectBalanceCriteria);
            return new EmptyResult();
        }

        public ActionResult ExportAgentBalance(GetDirectBalanceMasterDownlineCriteria getDirectBalanceMasterCriteria)
        {
            var result = this._directBalanceService.ExportBalanceMasterDownLineReport(getDirectBalanceMasterCriteria);
            return new EmptyResult();
        }

        public ActionResult ExportMemberBalance(GetDirectBalanceAgentDownlineCriteria getDirectBalanceAgentCriteria)
        {
            var result = this._directBalanceService.ExportBalanceAgentDownLineReport(getDirectBalanceAgentCriteria);
            return new EmptyResult();
        }

        public JsonResult GetCurrencies()
        {
            var result = this._directBalanceService.GetCurrencies();
            return Json(result);
        }
    }
}
