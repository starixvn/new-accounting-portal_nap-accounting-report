// -----------------------------------------------------------------------
// <copyright file="ReportingServicesController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.03.11">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Controllers
{
    using System.Configuration;
    using System.IO;
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Special.Criteria.ReportingServices;
    using Fanex.AccountingReports.Special.Services.Implement.ReportingServices;
    using Fanex.AccountingReports.Special.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    /// <summary>
    /// Reporting services controller.
    /// </summary>
    public class ReportingServicesController : BaseController
    {
        private IReportingService _reportingService;

        public ReportingServicesController()
            : this(null)
        {
        }

        public ReportingServicesController(IReportingService reportingService)
        {
            _reportingService = reportingService ?? new ReportingService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.ReportingServices, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Special/Views/ReportingServices/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetReport(GetReportCriteria criteria)
        {
            var result = _reportingService.GetReport(criteria);
            var jsonResult = Json(result);
            jsonResult.MaxJsonLength = int.MaxValue;
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        /// <summary>
        /// Downloads the specified criteria.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>File download.</returns>
        public ActionResult Download(GetReportCriteria criteria)
        {
            var defaultPath = ConfigurationManager.AppSettings["ReportingServicesPath"];
            var path = defaultPath + "\\" + criteria.Path;
            byte[] fileBytes = System.IO.File.ReadAllBytes(path);
            string fileName = Path.GetFileName(path);
            string fileExtensions = Path.GetExtension(path);
            string mineType = _reportingService.GetMimeType(fileExtensions);
            var contentResult = File(fileBytes, mineType, fileName);
            return contentResult;
        }
    }
}