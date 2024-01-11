namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameMonthlyWinLoss;
    using Fanex.AccountingReports.Gaming.Services.Implement.NumberGameMonthlyWinLoss;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    public class NumberGameMonthlyWinLossController : BaseController
    {
        private readonly NumberGameMonthlyWinLossService _numberGameMonthlyWinLossService;
        private readonly NumberGameMonthlyWinLossExportDataService _numberGameMonthlyWinLossExportDataService;

        public NumberGameMonthlyWinLossController()
        {
            this._numberGameMonthlyWinLossService = new NumberGameMonthlyWinLossService();
            this._numberGameMonthlyWinLossExportDataService = new NumberGameMonthlyWinLossExportDataService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberGameMonthlyWinloss, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/NumberGameMonthlyWinLoss/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetNumberGameMonthlyWinLoss(NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria)
        {
            var result = this._numberGameMonthlyWinLossService.GetNumberGameMonthlyWinLoss(getNumberGameMonthlyWinLossCriteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportReport(NumberGameMonthlyWinLossCriteria getNumberGameMonthlyWinLossCriteria)
        {
            this._numberGameMonthlyWinLossExportDataService.ExportNumberGameMonthlyWinLossReport(getNumberGameMonthlyWinLossCriteria);
            return new EmptyResult();
        }
    }
}
