// -----------------------------------------------------------------------
// <copyright file="NumberGameFinancialController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.04">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Services.Implement.NumberGameFinancial;
    using Fanex.AccountingReports.Gaming.Services.Interface.NumberGameFinancial;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Number Game Financial Report Controller.
    /// </summary>
    public class NumberGameFinancialController : BaseController
    {
        private readonly INumberGameFinancialService _numberGameFinancialService;
        private readonly INumberGameFinancialExportDataService _numberGameFinancialExportDataService;

        public NumberGameFinancialController()
        {
            this._numberGameFinancialService = new NumberGameFinancialService();
            this._numberGameFinancialExportDataService = new NumberGameFinancialExportDataService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberGameFinancial, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetNumberGameFinancial(GetNumberGameFinancialCriteria getNumberGameFinancialCriteria)
        {
            var result = this._numberGameFinancialService.GetNumberGameFinancial(getNumberGameFinancialCriteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportReport(GetNumberGameFinancialCriteria getNumberGameFinancialCriteria)
        {
            this._numberGameFinancialExportDataService.ExportNumberGameFinancial(getNumberGameFinancialCriteria);
            return new EmptyResult();
        }
    }
}