// --------------------------------------------------------------------------
// <copyright file="LegendManagementController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.LegendManagement.Controllers
{
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.LegendManagement.Criteria;
    using Fanex.AccountingReports.LegendManagement.Services.Impls;
    using Fanex.AccountingReports.LegendManagement.Services.Interfaces;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using System.Collections.Generic;
    using System.Web.Mvc;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LegendManagementController : BaseController
    {
        private readonly ILegendManagementService _legendService;

        public LegendManagementController()
            : this(null)
        {
        }

        public LegendManagementController(ILegendManagementService reportService)
        {
            this._legendService = reportService ?? new LegendManagementService();
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LegendManagement, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/LegendManagement/Views/LegendManagement/Index.cshtml");
        }

        [HttpPost]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LegendManagement, Policy = Constant.Policy.Read)]
        public JsonResult GetMenuList(GetSystemMenuCriteria criteria)
        {
            var menuList = this._legendService.GetMenuList(criteria);
            return Json(menuList, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LegendManagement, Policy = Constant.Policy.Host)]
        public JsonResult SaveReportLegend(SaveReportLegendCriteria criteria)
        {
            criteria.ConfigPath = Server.MapPath("~/web.config");
            var result = this._legendService.SaveReportLegend(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetReportLegend(GetReportLegendCriteria criteria)
        {
            var legendContent = this._legendService.GetReportLegend(criteria);
            return Json(legendContent, JsonRequestBehavior.AllowGet);
        }

        public string GetJsonpReportLegend(GetReportLegendCriteria criteria)
        {
            var legendContent = this._legendService.GetJsonpReportLegend(criteria);
            return StringExtensions.JsonpFormat(criteria.CallBackFunction, legendContent);
        }

        /// <summary>
        /// Uploads the image.
        /// </summary>
        /// <returns>Upload result.</returns>
        public ActionResult UploadImage()
        {
            var criteria = new UploadFileCriteria
            {
                File = Request.Files[0],
                SavePath = Server.MapPath("~/Areas/LegendManagement/Views/LegendManagement/UploadedImage"),
                FileName = Request.Form["fileName"]
            };

            var fileName = this._legendService.UploadImage(criteria);
            return Json(fileName, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LegendManagement, Policy = Constant.Policy.Host)]
        public JsonResult MakeInlineImages(List<string> imagesPath)
        {
            var inlineImages = this._legendService.MakeInlineImages(imagesPath, Server.MapPath("~/Areas/LegendManagement/Views/LegendManagement/UploadedImage"));
            return Json(inlineImages, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LegendManagement, Policy = Constant.Policy.Host)]
        public JsonResult SaveReportLegendMapping(SaveReportLegendMappingCriteria criteria)
        {
            var result = this._legendService.SaveReportLegendMapping(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [NapAuth(FunctionKey = AccReportFunctionKeys.LegendManagement, Policy = Constant.Policy.Read)]
        public JsonResult GetInputedLegendReport(GetInputedLegendReportCriteria criteria)
        {
            var result = this._legendService.GetInputedLegendReport(criteria);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
