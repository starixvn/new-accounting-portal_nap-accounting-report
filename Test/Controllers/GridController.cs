// --------------------------------------------------------------------------
// <copyright file="GridController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Test.Controllers
{
    using System;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria.TotalTransactions;
    using Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.AccountingReports.Test.Services.Implement;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GridController : BaseController
    {
        private readonly IWinLossByLicenseeService _reportService1;
        private readonly ITotalTransactionsService _reportService2;
        private readonly ExternalDailyWinlostService _reportService;

        public GridController()
        {
            this._reportService1 = new WinLossByLicenseeService();
            this._reportService2 = new TotalTransactionsService();
            this._reportService = new ExternalDailyWinlostService();
        }

        public ActionResult DemoGrid()
        {
            return View();
        }

        public ActionResult WLByLicensee()
        {
            return View();
        }

        /// <summary>
        /// Gets the currency report.
        /// </summary>
        /// <returns>The currency report.</returns>
        [HttpPost]
        public JsonResult GetCurrencyReport()
        {
            GetCurrencyReportCriteria criteria = new GetCurrencyReportCriteria
                {
                    ConvertToCurrency = 0,
                    Currency = 0,
                    CurrentPage = 1,
                    Drawfilter = 1,
                    FromDate = DateTime.Parse("12/1/2012"),
                    ToDate = DateTime.Parse("12/31/2012"),
                    ItemsPerPage = 50,
                    LicenseeID = 1,
                    ReportType = 1,
                    ShowBA = true,
                    ShowLC = true,
                    ShowNG = true,
                    ShowRB = true,
                    ShowSB = true,
                    ShowVS = false
                };
            var currencyReport = this._reportService1.GetCurrencyReport(criteria);
            return Json(currencyReport, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TotalTransactionsPage()
        {
            return View();
        }

        /// <summary>
        /// Gets the total trans report.
        /// </summary>
        /// <returns>The total trans report.</returns>
        [HttpPost]
        public JsonResult GetTotalTransReport()
        {
            GetTotalTransactionsCriteria criteria = new GetTotalTransactionsCriteria
            {
                BALicensee = "1,4,2,14,7,19,9,6,21,10",
                BALicenseeName = "12Bet,alog,asiabet88,Bodog,DELA,Macaubet,MayFair,TLC,VCasia,Zzun88",
                FromDate = DateTime.Parse("12/01/2012"),
                ToDate = DateTime.Now,
                ShowLC = true,
                ShowNG = true,
                ShowRB = true,
                ShowSB = true,
                ShowVS = true,
                NormalLicensee = "1,4,2,14,6,22,23,20,24,25,19,16,17,12,13,7,3,9,8,10,18,21",
                NormalLicenseeName = "12Bet,alog,asiabet88,Bodog," +
                "TLC,Aoncash,Bwinning,Ibobet," +
                "ITCBet,Livebet365,Macaubet,SG8," +
                "Tycoon,HAIFA,IBC,DELA," +
                "mansion88,MayFair,SunGame,Zzun88," +
                "18bb,VCasia"
            };

            var transactionReport = this._reportService2.GetTotalTransactions(criteria);
            return Json(transactionReport, JsonRequestBehavior.AllowGet);
        }

        public ActionResult DemoOnlyGrid()
        {
            return View();
        }

        public ActionResult ExternalReport()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetDailyWinlost()
        {
            var result = _reportService.GetDailyWinLoss();
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
