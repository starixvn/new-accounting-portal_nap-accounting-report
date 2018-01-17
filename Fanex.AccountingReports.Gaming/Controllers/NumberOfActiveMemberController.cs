namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.NumberOfActiveMember;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.AccountingReports.Gaming.Services.Implement;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.Excel;
    using Fanex.Excel.Extensions;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Number of Active Member Controller.
    /// </summary>
    public class NumberOfActiveMemberController : BaseController
    {
        /// <summary>
        /// Report Service.
        /// </summary>
        private readonly INumberOfActiveMemberService _reportService;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberOfActiveMemberController" /> class.
        /// </summary>
        public NumberOfActiveMemberController()
        {
            this._reportService = new NumberOfActiveMemberService();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumberOfActiveMemberController" /> class.
        /// </summary>
        /// <param name="reportService">The Report Service.</param>
        public NumberOfActiveMemberController(INumberOfActiveMemberService reportService)
        {
            this._reportService = reportService ?? new NumberOfActiveMemberService();
        }

        /// <summary>
        /// The Index.
        /// </summary>
        /// <returns>The index view.</returns>
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

        /// <summary>
        /// Get Report.
        /// </summary>
        /// <param name="criteria">The Criteria.</param>
        /// <returns>The JSON Object.</returns>
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

        /// <summary>
        /// Export Report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The empty result.</returns>
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
