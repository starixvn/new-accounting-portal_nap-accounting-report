namespace Fanex.AccountingReports.Licensee.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Web.Controllers;
    using Fanex.AccountingReports.Licensee.Criteria;
    using Fanex.AccountingReports.Licensee.Services.Implement;
    using Fanex.AccountingReports.Licensee.Services.Interface;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LicenseeController : BaseController
    {
        private readonly ILicenseeSiteService _licenseeSiteService;

        public LicenseeController()
        {
            _licenseeSiteService = new LicenseeSiteService();
        }

        [HttpPost]
        public JsonResult GetCurrenciesByLicensee(GetCurrenciesByLicenseeCriteria criteria)
        {
            var currencies = this._licenseeSiteService.GetCurrenciesByLicensee(criteria);
            return Json(currencies, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAllLicensees()
        {
            var licensees = this._licenseeSiteService.GetLicenseeSiteList();
            return Json(licensees, JsonRequestBehavior.AllowGet);
        }
    }
}
