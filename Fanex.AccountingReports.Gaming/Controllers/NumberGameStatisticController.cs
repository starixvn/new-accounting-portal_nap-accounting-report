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

        public NumberGameStatisticController()
            : this(null)
        {
        }

        public NumberGameStatisticController(INumberGameStatisticService service)
        {
            this._service = service ?? new NumberGameStatisticService();
        }

        [NapAuth(FunctionKey = AccReportFunctionKeys.NumberGameStatistic, Policy = Constant.Policy.Read)]
        public ActionResult Index()
        {
            return View("~/Areas/Gaming/Views/NumberGameStatistic/Index.cshtml");
        }

        [HttpPost]
        [HandleAjaxError]
        public ActionResult GetReport(GetNumberGameStatisticCriteria criteria)
        {
            var report = _service.GetReport(criteria);
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ExportReport(GetNumberGameStatisticCriteria criteria)
        {
            _service.ExportReport(criteria);
            return new EmptyResult();
        }
    }
}