// -----------------------------------------------------------------------
// <copyright file="MatchWinLossSingleBetController.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.WinLoss.Criteria.MatchWinLossSingleBet;
    using Fanex.AccountingReports.WinLoss.Services.Implement.MatchWinLossSingleBet;
    using Fanex.AccountingReports.WinLoss.Services.Interface.MatchWinLossSingleBet;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// The controller.
    /// </summary>
    public class MatchWinLossSingleBetController : BaseController
    {
        private readonly IMatchWinLossSingleBetService _reportService;

        public MatchWinLossSingleBetController()
                : this(null)
        {
        }

        public MatchWinLossSingleBetController(IMatchWinLossSingleBetService service)
        {
            this._reportService = service ?? new MatchWinLossSingleBetService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.MatchWinLossSingleBet, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/WinLoss/Views/MatchWinLossSingleBet/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetReport(GetMatchWinLossSingleBetCriteria criteria)
        {
            var report = this._reportService.GetReport(criteria);
            var jsonReport = Json(report);
            jsonReport.MaxJsonLength = int.MaxValue;
            return jsonReport;
        }

        public EmptyResult ExportReport(GetMatchWinLossSingleBetCriteria criteria)
        {
            this._reportService.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}
