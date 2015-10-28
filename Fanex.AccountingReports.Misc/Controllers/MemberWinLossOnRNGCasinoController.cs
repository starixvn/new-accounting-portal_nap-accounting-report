// -----------------------------------------------------------------------
// <copyright file="MemberWinLossOnRNGCasinoController.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Misc.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.MemberWinLossOnRNGCasino;
    using Fanex.AccountingReports.Misc.Services.Implement;
    using Fanex.AccountingReports.Misc.Services.Interface;
    using Fanex.Common;
    using Fanex.Excel.Extensions;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using NPOI.SS.UserModel;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MemberWinLossOnRNGCasinoController : BaseController
    {
        private IMemberWinLossOnRNGCasinoService _service;

        public MemberWinLossOnRNGCasinoController()
            : this(null)
        {
        }

        public MemberWinLossOnRNGCasinoController(IMemberWinLossOnRNGCasinoService service)
        {
            _service = service ?? new MemberWinLossOnRNGCasinoService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MemberWinLossOnRNGCasino, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Misc/Views/MemberWinLossOnRNGCasino/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetRecentReport(MemberWinLossOnRNGCasinoRecentCriteria criteria)
        {
            DateTime lastHour = ReportService.ConvertTimeZone(DateTime.Now.AddHours(-1));

            criteria.FromDate = Converter.ToDateTime(lastHour.ToString("yyyy-MM-dd HH:00:00"));
            criteria.ToDate = Converter.ToDateTime(lastHour.ToString("yyyy-MM-dd HH:59:59.997")); 
            var result = _service.GetRecentReport(criteria);
            return new JsonNetResult(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetNextTimeRefreshed()
        {            
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("NextTimeRefreshed", _service.GetNextRefreshTime().ToString("HH:mm") + " GMT-4");
            data.Add("NumOfMember", CacheManager.Get<int>(MemberWinLossOnRNGCasinoService.NumOfMemberKey));

            return new JsonNetResult(data);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetHistoryReport(MemberWinLossOnRNGCasinoHistoricalCriteria criteria)
        {
            criteria.ToDate = Converter.ToDateTime(criteria.ToDate.ToString("yyyy-MM-dd HH:mm:ss.997")); 
            var result = _service.GetHistoryReport(criteria);
            return new JsonNetResult(result);
        }

        public ActionResult GetLastRefreshTime()
        {
            var result = _service.GetLastRefreshTime();            
            DateTime now = DateTime.Now;
            DateTime serverTime = ReportService.ConvertTimeZone(now);

            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("Last Refreshed Time", result.ToString("yyyy-MM-dd HH:mm:ss"));
            data.Add("Now (GMT-4)", serverTime.ToString("yyyy-MM-dd HH:mm:ss"));
            data.Add("Local Time", now.ToString("yyyy-MM-dd HH:mm:ss"));

            return new JsonNetResult(data);
        }

        public ActionResult SetLastRefreshTime(DateTime? time)
        {
            _service.SetLastRefreshTime(time);
            var lastTime = time.HasValue ? time.Value.ToString("yyyy-MM-dd HH:mm:ss") : string.Empty;
            return new JsonNetResult("Last Refreshed Time has been set: " + lastTime);
        }

        public EmptyResult ExportRecentReport(MemberWinLossOnRNGCasinoRecentCriteria criteria)
        {
            IWorkbook workbook = _service.GetRecentReportWorkbook(criteria);
            NpoiExcelExtensions.ResponseStreamExcel(
                workbook,
                _service.GetReportName(criteria));
            return new EmptyResult();
        }

        public EmptyResult ExportHistoryReport(MemberWinLossOnRNGCasinoHistoricalCriteria criteria)
        {
            IWorkbook workbook = _service.GetHistoryReportWorkbook(criteria);
            NpoiExcelExtensions.ResponseStreamExcel(
                workbook,
                _service.GetReportName(criteria));
            return new EmptyResult();
        }
    }
}