// --------------------------------------------------------------------------
// <copyright file="TotalTransactionsController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.5">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria.TotalTransactions;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TotalTransactionsController : BaseController
    {
        private readonly ITotalTransactionsService _reportService;

        public TotalTransactionsController()
            : this(null)
        {
        }

        public TotalTransactionsController(ITotalTransactionsService reportService)
        {
            this._reportService = reportService ?? new TotalTransactionsService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.TotalTransactionsByLicensee, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View(new GetTotalTransactionsCriteria());
        }

        [HttpPost]
        public JsonResult Index(GetTotalTransactionsCriteria criteria)
        {
            var transactionReport = this._reportService.GetTotalTransactions(criteria);
            return Json(transactionReport, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllResellers()
        {
            var reseller = this._reportService.GetAllResellers();
            return Json(reseller, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Export(GetTotalTransactionsCriteria criteria)
        {
            this._reportService.Export(criteria);
            return new EmptyResult();
        }
    }
}