namespace Fanex.AccountingReports.Misc.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Misc.Criteria.DuplicateTickets;
    using Fanex.AccountingReports.Misc.Services.Implement.DuplicateTickets;
    using Fanex.AccountingReports.Misc.Services.Interface.DuplicateTickets;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DuplicateTicketsController : BaseController
    {
        private readonly IDuplicateTicketsService _reportService;

        public DuplicateTicketsController()
        {
            this._reportService = new DuplicateTicketsService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.DuplicateTickets, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Misc/Views/DuplicateTickets/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetDuplicateTicketReport(GetDuplicateTicketsCriteria criteria)
        {
            var result = this._reportService.GetDuplicateTicketByUserName(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        public ActionResult ExportDuplicateTicketReport(GetDuplicateTicketsCriteria criteria)
        {
            this._reportService.ExportDuplicateTicketReport(criteria);
            return new EmptyResult();
        }
    }
}
