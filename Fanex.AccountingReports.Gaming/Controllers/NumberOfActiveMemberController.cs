namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using AccountingReports.Core.Web;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.NumberOfActiveMember;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Implement;
    using Fanex.Excel;
    using Fanex.Excel.Extensions;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    public class NumberOfActiveMemberController : BaseController
    {
        private readonly NumberOfActiveMemberService _reportService;

        public NumberOfActiveMemberController()
        {
            this._reportService = new NumberOfActiveMemberService();
        }

        [HttpGet]
        [UserActivityFilter]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberofActiveMembers, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return this.View();
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetProduct()
        {
            var products = _reportService.GetProduct();
            return Json(products);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonNetResult GetReport(GetNumberOfActiveMemberCriteria criteria)
        {
            var report = this._reportService.GetReport(criteria);
            return new JsonNetResult(report);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonNetResult SearchNumberOfActiveMemberByProduct(SearchNumberActiveMemberCriteria criteria)
        {
            var report = this._reportService.SearchNumberOfActiveMemberByProduct(criteria);
            return new JsonNetResult(report);
        }

        public ActionResult ExportReport(GetNumberOfActiveMemberCriteria criteria)
        {
            string reportName = string.Format(
                GamingResources.NumberOfActiveMember_ReportName,
                criteria.FromDate.ToShortFormatedDateWithSlash().Replace("/", "_"),
                criteria.ToDate.ToShortFormatedDateWithSlash().Replace("/", "_"));
            string sheetName = GamingResources.NumberOfActiveMember_SheetName;
            var result = new FanexExcel(_reportService.GetReportFromCache(criteria));
            NpoiExcelExtensions.ResponseStreamExcel(result.ToExcel2003(sheetName), reportName);
            return new EmptyResult();
        }
    }
}
