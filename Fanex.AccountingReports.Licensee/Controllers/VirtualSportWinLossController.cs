// --------------------------------------------------------------------------
// <copyright file="VirtualSportWinLossController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Robert.Tran" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Criteria.VirtualSportWinLoss;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;
    using Newtonsoft.Json;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [NapSuperAuth]
    public class VirtualSportWinLossController : BaseController
    {
        private readonly IVirtualSportWinLossService _reportService;
        private JsonSerializerSettings _settings = null;

        public VirtualSportWinLossController()
            : this(null)
        {
        }

        public VirtualSportWinLossController(IVirtualSportWinLossService reportService)
        {
            _reportService = reportService ?? new VirtualSportWinLossService();
            _settings = new JsonSerializerSettings();
            _settings.NullValueHandling = NullValueHandling.Ignore;
            _settings.Formatting = Formatting.Indented;
        }

        [HttpGet]
        [NapAuth(FunctionKey = AccReportFunctionKeys.VirtualSportsWinLossByLicensee, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult SelAllLicensee()
        {
            var result = _reportService.GetAllLicensees(new GetLicenseeByResellerCriteria());
            return Json(result);
        }

        [HttpPost]
        public JsonResult SelSporttype()
        {
            var result = _reportService.GetSportType();
            return Json(result);
        }

        [HttpPost]
        public string SelVirtualSportWinLoss(GetVirtualSportWinLossCriteria param)
        {
            var result = _reportService.SelVirtualSportWinLoss(param);
            var jsonObj = JsonConvert.SerializeObject(result, _settings);
            return jsonObj;
        }

        [HttpGet]
        public ActionResult ExportGameWinLossReport(GetVirtualSportWinLossCriteria param)
        {
            _reportService.ExportExcelVirtualSportsWinLossReport(param);
            return new EmptyResult();
        }
    }
}