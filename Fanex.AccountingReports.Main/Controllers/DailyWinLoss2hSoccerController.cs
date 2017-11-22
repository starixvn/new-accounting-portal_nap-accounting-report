// --------------------------------------------------------------------------
// <copyright file="DailyWinLoss2hSoccerController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccer;
    using Fanex.AccountingReports.Main.Services.Implement.DailyWinLoss2hSoccer;
    using Fanex.AccountingReports.Main.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyWinLoss2hSoccerController : BaseController
    {
        private readonly IDailyWinLoss2hSoccerService _reportService;

        public DailyWinLoss2hSoccerController()
        {
            this._reportService = new DailyWinLoss2hSoccerService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.DailyWinLoss2hSoccer, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonWithoutNullField GetMain2hReport(GetMain2hReportCriteria criteria)
        {
            var result = this._reportService.GetMain2hReport(criteria);
            var jsonResult = new JsonWithoutNullField(result);
            return jsonResult;
        }

        //// ExportExcel

        public ActionResult ExportMain2hReport(GetMain2hReportCriteria criteria)
        {
            this._reportService.ExportMain2hReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportReport_DirectFilter(GetMain2hReportCriteria criteria)
        {
            this._reportService.ExportReport_DirectFilter(criteria);
            return new EmptyResult();
        }

        //// ExportExcel
    }
}
