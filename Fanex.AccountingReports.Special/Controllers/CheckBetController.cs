namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Web.Mvc;
    using Core.Exceptions;
    using Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.CheckBet;
    using Fanex.AccountingReports.Special.Services.Implement;
    using Fanex.AccountingReports.Special.Services.Interface;
    using NAP.Core.Business;
    using NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CheckBetController : BaseController
    {
        private readonly ICheckBetService _checkBetService;

        public CheckBetController()
        {
            this._checkBetService = new CheckBetService();
        }

        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.CheckBet, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SelCheckedBet(CheckBetCriteria checkBetCriteria)
        {
            var result = this._checkBetService.GetCheckedBet(checkBetCriteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SelNormalTickets(CheckBetCriteria checkBetCriteria)
        {
            var result = this._checkBetService.GetNormalChangedTicket(checkBetCriteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult SelRejectTickets(CheckBetCriteria checkBetCriteria)
        {
            var result = this._checkBetService.GetRejectChangedTicket(checkBetCriteria);
            return Json(result);
        }
    }
}
