namespace Fanex.AccountingReports.Gaming.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameStatistic;
    using Fanex.AccountingReports.Gaming.Services.Implement;
    using Fanex.AccountingReports.Gaming.Services.Interface;
    using Fanex.NAP.Core.Business;
    using Fanex.NAP.Core.Web.Attribute;

    public class NumberGameStatisticController : BaseController
    {
        private INumberGameStatisticService _service;
        private INumberGameStatisticExportService _exportService;

        public NumberGameStatisticController()
            : this(null, null)
        {
        }

        public NumberGameStatisticController(INumberGameStatisticService service, INumberGameStatisticExportService exportService)
        {
            this._service = service ?? new NumberGameStatisticService();
            this._exportService = exportService ?? new NumberGameStatisticExportService();
        }

        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberGameStatistic, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/NumberGameStatistic/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetReportByDaily(GetNumberGameStatisticCriteria criteria)
        {
            var report = _service.GetNumberGameStatisticByDaily(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetReportByUserName(GetNumberGameStatisticCriteria criteria)
        {
            var report = _service.GetNumberGameStatisticByUserName(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetReportByLeague(GetNumberGameStatisticCriteria criteria)
        {
            var report = _service.GetNumberGameStatisticByLeague(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetReportByBettype(GetNumberGameStatisticCriteria criteria)
        {
            var report = _service.GetNumberGameStatisticByBettype(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportReportByDaily(GetNumberGameStatisticCriteria criteria)
        {
            _exportService.ExportReportByDaily(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportReportByUserName(GetNumberGameStatisticCriteria criteria)
        {
            _exportService.ExportReportByUserName(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportReportByLeague(GetNumberGameStatisticCriteria criteria)
        {
            _exportService.ExportReportByLeague(criteria);
            return new EmptyResult();
        }

        public ActionResult ExportReportByBettype(GetNumberGameStatisticCriteria criteria)
        {
            _exportService.ExportReportByBettype(criteria);
            return new EmptyResult();
        }
    }
}