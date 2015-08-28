// -----------------------------------------------------------------------
// <copyright file="GreyhoundAUSToteController.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Racing.Criteria.ForTote.GreyhoundAUSTote;
    using Fanex.AccountingReports.Racing.Services.Implement;
    using Fanex.AccountingReports.Racing.Services.Implement.ForTote.GreyhoundAUSTote;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GreyhoundAUSToteController : BaseController
    {
        private readonly ToteDailyWinLossService _greyhoundWinLossService;
        private readonly ToteDailyWinLossExcelService _greyhoundExcelService;

        public GreyhoundAUSToteController()
        {
            _greyhoundWinLossService = new GreyhoundWinLossAUSToteService();
            _greyhoundExcelService = new GreyhoundWinLossAUSToteExcelService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.GreyhoundForAustTote, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Racing/Views/GreyhoundAUSTote/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetRacingDailyWinLoss(GetMainGreyhoundAUSToteCriteria criteria)
        {
            var result = _greyhoundWinLossService.GetRacingDailyWinLossMain(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetRacingDailyWinLossDetail(GetDetailGreyhoundAUSToteCriteria criteria)
        {
            var result = _greyhoundWinLossService.GetRacingDailyWinLossDetail(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public EmptyResult ExportExcelRacingWinLossMain(GetMainGreyhoundAUSToteCriteria criteria)
        {
            _greyhoundExcelService.ExportExcelRacingDailyMain(criteria);
            return new EmptyResult();
        }

        public EmptyResult ExportExcelRacingWinLossDetail(GetDetailGreyhoundAUSToteCriteria criteria)
        {
            _greyhoundExcelService.ExportExcelRacingDailyDetail(criteria);
            return new EmptyResult();
        }
    }
}
