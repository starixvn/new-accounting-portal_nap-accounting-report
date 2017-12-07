namespace Fanex.AccountingReports.Core.Web.Controllers
{
    using System.Web.Mvc;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Exceptions;
    using Fanex.AccountingReports.Core.Services.Impl;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ReportHelperController : BaseController
    {
        /// <summary>
        /// Gets the user name suggestion.
        /// </summary>
        /// <param name="excludeTestAccounts">If set to <c>true</c> [exclude test accounts].</param>
        /// <param name="query">The query.</param>
        /// <returns>The user name suggestion.</returns>
        [HandleAjaxError]
        public JsonResult UserNameSuggestion(bool excludeTestAccounts, string query)
        {
            var criteria = new UserNameSuggestionCriteria
            {
                Username = query,
                ExcludeTestAccounts = excludeTestAccounts
            };

            var result = ReportService.UserNameSuggestion(criteria);
            var jsonResult = Json(new
            {
                query = result.QueryValue,
                suggestions = result.Suggestions,
                data = result.DataInfo
            });

            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }

        /// <summary>
        /// Gets the currency by licensee.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The currency by licensee.</returns>
        [HttpPost]
        [HandleAjaxError]
        public JsonResult GetCurrencyByLicensee(GetCurrencyByLicenseeCriteria criteria)
        {
            var result = ReportService.GetCurrencyByLicensee(criteria);
            var jsonResult = Json(result);
            jsonResult.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return jsonResult;
        }
    }
}
