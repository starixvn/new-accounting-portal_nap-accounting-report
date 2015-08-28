// -----------------------------------------------------------------------
// <copyright file="RacingAUSToteController.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Racing.Criteria.ForTote.RacingAUSTote;
    using Fanex.AccountingReports.Racing.Services.Implement.RacingAUSTote;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Create Racing AUS Tote Controller.
    /// </summary>
    public class RacingAUSToteController : BaseController
    {
        private readonly RacingWinLossAUSToteService _racingWinLossService;
        private readonly RacingDailyAUSToteExcelService _racingExcelService;

        public RacingAUSToteController()
        {
            _racingWinLossService = new RacingWinLossAUSToteService();
            _racingExcelService = new RacingDailyAUSToteExcelService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.HorseRacingForAustTote, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Racing/Views/RacingAUSTote/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetRacingDailyWinLoss(GetMainRacingAUSToteCriteria criteria)
        {
            var result = _racingWinLossService.GetRacingDailyWinLossMain(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetRacingDailyWinLossDetail(GetDetailRacingAUSToteCriteria criteria)
        {
            var result = _racingWinLossService.GetRacingDailyWinLossDetail(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public EmptyResult ExportExcelRacingWinLossMain(GetMainRacingAUSToteCriteria criteria)
        {
            _racingExcelService.ExportExcelRacingDailyMain(criteria);
            return new EmptyResult();
        }

        public EmptyResult ExportExcelRacingWinLossDetail(GetDetailRacingAUSToteCriteria criteria)
        {
            _racingExcelService.ExportExcelRacingDailyDetail(criteria);
            return new EmptyResult();
        }
    }
}
