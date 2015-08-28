// -----------------------------------------------------------------------
// <copyright file="DailyTurnoverController.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.DailyTurnover;
    using Fanex.AccountingReports.Special.Services.Implement.DailyTurnover;
    using Fanex.AccountingReports.Special.Services.Interface.DailyTurnover;
    using Fanex.AccountingReports.Special.Services.Interface.IDailyTurnover;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// The controller of Daily Turnover.
    /// </summary>
    [NapSuperAuth]
    public class DailyTurnoverController : BaseController
    {
        private readonly IDailyTurnoverService _reportService;

        private readonly IDailyDetailChangedTurnoverService _reportDetailService;

        private readonly IDailyDetailChangedTurnoverExportReportService _exportReportDetailService;

        public DailyTurnoverController()
            : this(null, null, null)
        { 
        }

        public DailyTurnoverController(
            IDailyTurnoverService reportService,
            IDailyDetailChangedTurnoverService reportDetailService,
            IDailyDetailChangedTurnoverExportReportService exportReportDetailService)
        {
            this._reportService = reportService ?? new DailyTurnoverService();
            this._reportDetailService = reportDetailService ?? new DailyDetailChangedTurnoverService();
            this._exportReportDetailService = exportReportDetailService ?? 
                                                new DailyDetailChangedTurnoverExportReportService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.DailyTurnover, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Special/Views/DailyTurnover/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetDailyTurnoverCriteria criteria)
        {
            var report = this._reportService.GetReport(criteria);
            return Json(report);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetDetailReport(GetDailyTurnoverChangedDetailCriteria criteria)
        {
            var detailReport = this._reportDetailService.GetDetailReport(criteria);
            return Json(detailReport);
        }

        public EmptyResult ExportReport(GetDailyTurnoverCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }

        public EmptyResult ExportDetailReport(GetDailyTurnoverChangedDetailCriteria criteria)
        {
            this._exportReportDetailService.ExportDetailReport(criteria);
            return new EmptyResult();
        }

        public EmptyResult ExportDetailDifferenceReport(string index, string fromDate)
        {
            this._exportReportDetailService.ExportDetailDifferenceReport(index, fromDate);
            return new EmptyResult();
        }
    }
}
