// -----------------------------------------------------------------------
// <copyright file="VirtualSportsWinLossController.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.VirtualSportsWinLoss;
    using Fanex.AccountingReports.Gaming.Services.Implement.VirtualSportWinLoss;
    using Fanex.AccountingReports.Gaming.Services.Interface.VirtualSportWinLoss;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// The controller.
    /// </summary>
    public class VirtualSportsWinLossController : BaseController
    {
        private readonly IVirtualSportsWinLossGetReportService _getReportService;

        private readonly IVirtualSportsWinLossDetailService _reportDetailService;

        private readonly IVirtualSportsWinLossExportReportService _exportReportService;

        public VirtualSportsWinLossController()
        {
            this._getReportService = new VirtualSportsWinLossGetReportService();
            this._reportDetailService = new VirtualSportsWinLossDetailService();
            this._exportReportService = new VirtualSportsWinLossExportReportService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.VirtualSportsWinLossGaming,
                 Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/VirtualSportsWinLoss/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetVirtualSportsWinLossCriteria criteria)
        {
            var report = this._getReportService.GetReport(criteria);
            var jsonResult = Json(report);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetDetailReport(GetVirtualSportsWinLossDetailCriteria criteria)
        {
            var detailReport = this._reportDetailService.GetDetailReport(criteria);
            var jsonResult = Json(detailReport);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetSportsCollection()
        {
            var sportsCollection = this._getReportService.GetSportsCollection();
            return Json(sportsCollection);
        }

        public EmptyResult ExportReport(GetVirtualSportsWinLossCriteria criteria)
        {
            this._exportReportService.ExportReport(criteria);
            return new EmptyResult();
        }

        public EmptyResult ExportReportDetail(GetVirtualSportsWinLossDetailCriteria criteria)
        {
            this._reportDetailService.ExportDetailReport(criteria);
            return new EmptyResult();
        }
    }
}
