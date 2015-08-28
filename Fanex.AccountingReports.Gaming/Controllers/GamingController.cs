// --------------------------------------------------------------------------
// <copyright file="GamingController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria;
    using Fanex.AccountingReports.Gaming.Services.Implement;
    using Fanex.AccountingReports.Gaming.Services.Interface;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GamingController : BaseController
    {
        private readonly IGamingService _reportService;

        public GamingController()
            : this(null)
        {
        }

        public GamingController(IGamingService reportService)
        {
            this._reportService = reportService ?? new GamingService();
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetLicensee_SupportBingo()
        {
            var result = this._reportService.GetLicensee_SupportBingo();
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetNumberGameCompanyList(GetNumberGameCompanyListCriteria criteria)
        {
            var result = _reportService.GetNumberGameCompanyList(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetNumberGameBetTypeList(GetNumberGameBetTypeListCriteria criteria)
        {
            var result = _reportService.GetNumberGameBetTypeList(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetLiveCasinoLicenseeList(GetLiveCasinoGameCompanyListCriteria criteria)
        {
            var result = _reportService.GetLiveCasinoLicenseeList(criteria);
            return Json(result);
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetLicenseesByProduct(GetLicenseesByProductCriteria criteria)
        {
            var result = _reportService.GetLicenseesByProduct(criteria);
            return Json(result);
        }
    }
}
