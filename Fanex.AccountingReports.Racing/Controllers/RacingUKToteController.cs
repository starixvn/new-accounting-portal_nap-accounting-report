namespace Fanex.AccountingReports.Racing.Controllers
{
    using System.Web.Mvc;
    using AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Racing.Criteria.ForTote.RacingUKTote;
    using Fanex.AccountingReports.Racing.Services.Implement.RacingUKTote;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RacingUKToteController : BaseController
    {
        private readonly RacingWinLossUKToteService _racingWinLossService;
        private readonly RacingDailyUKToteExcelService _racingExcelService;

        public RacingUKToteController()
        {
            _racingWinLossService = new RacingWinLossUKToteService();
            _racingExcelService = new RacingDailyUKToteExcelService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.HorseRacingForUKTote, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Racing/Views/RacingUKTote/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetRacingDailyWinLoss(GetMainRacingUKToteCriteria criteria)
        {
            var result = _racingWinLossService.GetRacingDailyWinLossMain(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetRacingDailyWinLossDetail(GetDetailRacingUKToteCriteria criteria)
        {
            var result = _racingWinLossService.GetRacingDailyWinLossDetail(criteria);
            var jsonResult = Json(result, JsonRequestBehavior.AllowGet);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public EmptyResult ExportExcelRacingWinLossMain(GetMainRacingUKToteCriteria criteria)
        {
            _racingExcelService.ExportExcelRacingDailyMain(criteria);
            return new EmptyResult();
        }

        public EmptyResult ExportExcelRacingWinLossDetail(GetDetailRacingUKToteCriteria criteria)
        {
            _racingExcelService.ExportExcelRacingDailyDetail(criteria);
            return new EmptyResult();
        }
    }
}
