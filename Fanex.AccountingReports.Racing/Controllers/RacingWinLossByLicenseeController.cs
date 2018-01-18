namespace Fanex.AccountingReports.Racing.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Models.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Services.Implement.RacingWinLossByLicensee;
    using Fanex.AccountingReports.Racing.Services.Interface.RacingWinLossByLicensee;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// The controller of Racing Win Loss By Licensee report.
    /// </summary>
    public class RacingWinLossByLicenseeController : BaseController
    {
        private readonly IRacingWinLossByLicenseeService _getRacingDailyReport;

        private readonly IRacingDetailBetListService _getDetailBetListService;

        private readonly IRacingGetListLicenseeService _getListLicenseeService;

        private readonly IRacingWinLossByLicenseeExportReportService _exportReportService;

        private readonly IRacingWinLossByLicenseeApply50PercentService _export50PercentReportService;

        public RacingWinLossByLicenseeController()
        {
            _getRacingDailyReport = new RacingWinLossByLicenseeService();
            _getDetailBetListService = new RacingDetailBetListService();
            _getListLicenseeService = new RacingGetListLicenseeService();
            _exportReportService = new RacingWinLossByLicenseeExportReportService();
            _export50PercentReportService = new RacingWinLossByLicenseeApply50PercentService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.RacingWinLossByLicensee, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Racing/Views/RacingWinLossByLicensee/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetRacingWinLossByLicensee(GetRacingDailyReportCriteria criteria)
        {
            RacingDailyReport report = this._getRacingDailyReport.GetReport(criteria);
            JsonResult jsonResult = Json(report);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetDetailBetList(GetDetailBetListCriteria criteria)
        {
            List<DetailBetList> detailBetList = this._getDetailBetListService.GetDetailBetList(criteria);
            JsonResult jsonResult = Json(detailBetList);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        public ActionResult GetLicensee(GetLicenseeCriteria criteria)
        {
            List<RacingLicensee> licensee = this._getListLicenseeService.GetListLicensee(criteria);
            return Json(licensee);
        }

        public EmptyResult ExportReport(GetRacingDailyReportCriteria criteria)
        {
            this._exportReportService.ExportReport(criteria);
            return new EmptyResult();
        }

        public EmptyResult Export50PercentReport(GetRacingDailyReportCriteria criteria)
        {
            this._export50PercentReportService.ExportReportApply50Percent(criteria);
            return new EmptyResult();
        }
    }
}
