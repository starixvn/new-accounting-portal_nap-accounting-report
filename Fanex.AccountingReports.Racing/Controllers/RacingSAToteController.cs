namespace Fanex.AccountingReports.Racing.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Racing.Criteria.ForTote.RacingSATote;
    using Fanex.AccountingReports.Racing.Services.Implement.RacingSATote;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Racing SA Tote Controller.
    /// </summary>
    public class RacingSAToteController : BaseController
    {
        private readonly RacingWinLossSAToteService _racingWinLossService;
        private readonly RacingDailySAToteExcelService _racingExcelService;

        public RacingSAToteController()
        {
            _racingWinLossService = new RacingWinLossSAToteService();
            _racingExcelService = new RacingDailySAToteExcelService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.HorseRacingForSATote, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Racing/Views/RacingSATote/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetRacingDailyWinLoss(GetMainRacingSAToteCriteria criteria)
        {
            var result = _racingWinLossService.GetRacingDailyWinLossMain(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetRacingDailyWinLossDetail(GetDetailRacingSAToteCriteria criteria)
        {
            var result = _racingWinLossService.GetRacingDailyWinLossDetail(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public EmptyResult ExportExcelRacingWinLossMain(GetMainRacingSAToteCriteria criteria)
        {
            _racingExcelService.ExportExcelRacingDailyMain(criteria);
            return new EmptyResult();
        }

        public EmptyResult ExportExcelRacingWinLossDetail(GetDetailRacingSAToteCriteria criteria)
        {
            _racingExcelService.ExportExcelRacingDailyDetail(criteria);
            return new EmptyResult();
        }
    }
}
