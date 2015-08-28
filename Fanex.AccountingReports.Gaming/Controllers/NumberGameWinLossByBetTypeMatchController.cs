// --------------------------------------------------------------------------
// <copyright file="NumberGameWinLossByBetTypeMatchController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Services.Implement.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Services.Interface.NumberGameWinLossByBetTypeMatch;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class NumberGameWinLossByBetTypeMatchController : BaseController
    {
        private readonly INumberGameWinLossByBetTypeMatchService _numberGameWinLossService;

        private readonly IWinLossByBetTypeService _winLossByBetTypeService;

        private readonly IWinLossByMatchService _winLossByMatchService;

        public NumberGameWinLossByBetTypeMatchController()
            : this(null, null, null)
        {
        }

        public NumberGameWinLossByBetTypeMatchController(
            INumberGameWinLossByBetTypeMatchService numberGameWinLossService,
            IWinLossByBetTypeService winLossByBetTypeService,
            IWinLossByMatchService winLossByMatchService)
        {
            _numberGameWinLossService = numberGameWinLossService ?? new NumberGameWinLossByBetTypeMatchService();
            _winLossByBetTypeService = winLossByBetTypeService ?? new WinLossByBetTypeService();
            _winLossByMatchService = winLossByMatchService ?? new WinLossByMatchService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberGameWinLoss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/NumberGameWinLossByBetTypeMatch/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCurrencyList()
        {
            var result = _numberGameWinLossService.GetCurrencyList();
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetWinLossByBetTypeMainReport(GetWinLossByBetTypeMainReportCriteria criteria)
        {
            var result = _winLossByBetTypeService.GetMainReport(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetWinLossByBetTypeDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria)
        {
            var result = _winLossByBetTypeService.GetDetailReport(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetWinLossByMatchMainReport(GetWinLossByMatchMainReportCriteria criteria)
        {
            var result = _winLossByMatchService.GetMainReport(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetWinLossByMatchDetailReport(GetWinLossByMatchDetailReportCriteria criteria)
        {
            var result = _winLossByMatchService.GetDetailReport(criteria);
            return Json(result);
        }

        public ActionResult ExportWinLossByBetTypeMainReport(GetWinLossByBetTypeMainReportCriteria criteria)
        {
            _winLossByBetTypeService.ExportMainReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportWinLossByBetTypeDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria)
        {
            _winLossByBetTypeService.ExportDetailReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportWinLossByMatchMainReport(GetWinLossByMatchMainReportCriteria criteria)
        {
            _winLossByMatchService.ExportMainReport(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportWinLossByMatchDetailReport(GetWinLossByMatchDetailReportCriteria criteria)
        {
            _winLossByMatchService.ExportDetailReport(criteria);
            return new EmptyResult();
        }
    }
}
