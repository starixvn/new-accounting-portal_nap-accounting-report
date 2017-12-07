namespace Fanex.AccountingReports.Main.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.AccountingReports.Main.Resources;
    using Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.Excel;
    using Fanex.Excel.Extensions;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyWinLoss2hSoccerMinorBettypeController : BaseController
    {
        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.DailyWinLoss2hSoccerMinorBettype, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Main/Views/DailyWinLoss2hSoccerMinorBettypes/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonNetResult GenerateMainReport(GetMainReportCriteria criteria)
        {
            MainReportService service = new MainReportService();
            return new JsonNetResult(service.GetReport(criteria));
        }

        public ActionResult ExportExcelMainReport(GetMainReportCriteria criteria)
        {
            MainReportService service = new MainReportService();
            FanexExcel result = new FanexExcel(service.GetReport(criteria, true));
            NpoiExcelExtensions.ResponseStreamExcel(result.ToExcel2003(), this.GetFileName(criteria.WinlostDate));
            return new EmptyResult();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonNetResult GenerateSoccerReport(GetSoccerReportCriteria criteria)
        {
            SportTypeReportService service = new SportTypeReportService();
            return new JsonNetResult(service.GetReport(criteria));
        }

        public ActionResult ExportExcelSoccerReport(GetSoccerReportCriteria criteria)
        {
            SportTypeReportService service = new SportTypeReportService();
            FanexExcel result = new FanexExcel(service.GetReport(criteria, true));
            NpoiExcelExtensions.ResponseStreamExcel(result.ToExcel2003(), this.GetFileName(criteria.WinlostDate));
            return new EmptyResult();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonNetResult GenerateBettypeReport(Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h.GetReportByBetTypeCriteria criteria)
        {
            criteria.SportType = 1;
            BetTypeReportService service = new BetTypeReportService();
            return new JsonNetResult(service.GetReport(criteria));
        }

        public ActionResult ExportExcelBettypeReport(Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h.GetReportByBetTypeCriteria criteria)
        {
            criteria.SportType = 1;
            BetTypeReportService service = new BetTypeReportService();
            FanexExcel result = new FanexExcel(service.GetReport(criteria, true));
            NpoiExcelExtensions.ResponseStreamExcel(result.ToExcel2003(), this.GetFileName(criteria.Winlostdate));
            return new EmptyResult();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonNetResult GenerateSiteReport(Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h.GetWinLossBySite_BetTypeCriteria criteria)
        {
            criteria.SportType = 1;
            SiteReportService service = new SiteReportService();
            return new JsonNetResult(service.GetReport(criteria));
        }

        public ActionResult ExportExcelSiteReport(Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h.GetWinLossBySite_BetTypeCriteria criteria)
        {
            criteria.SportType = 1;
            SiteReportService service = new SiteReportService();
            FanexExcel result = new FanexExcel(service.GetReport(criteria, true));
            NpoiExcelExtensions.ResponseStreamExcel(result.ToExcel2003(), this.GetFileName(criteria.WinLostDate));
            return new EmptyResult();
        }

        public string GetBettypeName(int bettype)
        {
            return CacheRef.GetBettype(bettype);
        }

        private string GetFileName(DateTime winlossDate)
        {
            return string.Format(MainResources.DailyWinLoss2h_ReportName, winlossDate.ToString("MM_dd_yyyy"));
        }
    }
}
