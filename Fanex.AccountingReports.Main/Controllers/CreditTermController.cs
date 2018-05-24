namespace Fanex.AccountingReports.Main.Controllers
{
    using System.Web.Mvc;
    using Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Main.Criteria.CreditTerm;
    using Fanex.AccountingReports.Main.Services.Implement.CreditTerm;
    using Fanex.AccountingReports.Main.Services.Interface.CreditTerm;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// The controller of Credit Term report.
    /// </summary>
    public class CreditTermController : BaseController
    {
        private ICreditTermService _getCreditReportService;

        private IInstallmentAmountService _getInstallmentAmountService;

        private ICreditTermExportExcelService _exportCreditReportService;

        private IInstallmentAmountExportExcelService _exportInstallmentAmountService;

        public CreditTermController()
        {
            this._getCreditReportService = new CreditTermService();
            this._getInstallmentAmountService = new InstallmentAmountService();
            this._exportCreditReportService = new CreditTermExportExcelService();
            this._exportInstallmentAmountService = new InstallmentAmountExportExcelService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.CreditTerm, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Main/Views/CreditTerm/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCreditTermReport(GetCreditTermCriteria criteria)
        {
            var report = this._getCreditReportService.GetCreditTermReport(criteria);
            var jsonResult = Json(report);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        [HttpPost]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.CreditTerm, Policy = Constant.Policy.Host)]
        [HandleAjaxError]
        public JsonResult EditRemark(GetRemarkEditedCriteria criteria)
        {
            var editedRemark = this._getCreditReportService.EditRemark(criteria);
            return Json(editedRemark);
        }

        public EmptyResult ExportCreditTermReport(GetCreditTermCriteria criteria)
        {
            this._exportCreditReportService.ExportCreditTermReport(criteria);
            return new EmptyResult();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetInstallmentAmountReport(GetInstallAmountCriteria criteria)
        {
            var report = this._getInstallmentAmountService.GetInstallmentAmountReport(criteria);
            var jsonResult = Json(report);
            jsonResult.MaxJsonLength = int.MaxValue;
            return jsonResult;
        }

        public EmptyResult ExportInstallmentAmountReport(GetInstallAmountCriteria criteria)
        {
            this._exportInstallmentAmountService.ExportInstallmentAmountReport(criteria);
            return new EmptyResult();
        }
    }
}
