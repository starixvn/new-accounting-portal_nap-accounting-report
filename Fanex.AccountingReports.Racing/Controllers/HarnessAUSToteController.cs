// -----------------------------------------------------------------------
// <copyright file="HarnessAUSToteController.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Racing.Criteria.ForTote.HarnessAUSTote;
    using Fanex.AccountingReports.Racing.Services.Implement;
    using Fanex.AccountingReports.Racing.Services.Implement.ForTote.HarnessAUSTote;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class HarnessAUSToteController : BaseController
    {
        private readonly ToteDailyWinLossService _greyhoundWinLossService;
        private readonly ToteDailyWinLossExcelService _greyhoundExcelService;

        public HarnessAUSToteController()
        {
            _greyhoundWinLossService = new HarnessWinLossAUSToteService();
            _greyhoundExcelService = new HarnessWinLossAUSToteExcelService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.HarnessForAustTote, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Racing/Views/HarnessAUSTote/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetRacingDailyWinLoss(GetMainHarnessAUSToteCriteria criteria)
        {
            var result = _greyhoundWinLossService.GetRacingDailyWinLossMain(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetRacingDailyWinLossDetail(GetDetailHarnessAUSToteCriteria criteria)
        {
            var result = _greyhoundWinLossService.GetRacingDailyWinLossDetail(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public EmptyResult ExportExcelRacingWinLossMain(GetMainHarnessAUSToteCriteria criteria)
        {
            _greyhoundExcelService.ExportExcelRacingDailyMain(criteria);
            return new EmptyResult();
        }

        public EmptyResult ExportExcelRacingWinLossDetail(GetDetailHarnessAUSToteCriteria criteria)
        {
            _greyhoundExcelService.ExportExcelRacingDailyDetail(criteria);
            return new EmptyResult();
        }
    }
}
