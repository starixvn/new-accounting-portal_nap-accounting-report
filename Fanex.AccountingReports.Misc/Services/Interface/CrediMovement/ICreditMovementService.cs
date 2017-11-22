namespace Fanex.AccountingReports.Misc.Services.Interface.CrediMovement
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Misc.Criteria.CreditMovement;
    using Fanex.AccountingReports.Misc.Models.CreditMovement;
    using Fanex.Excel.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICreditMovementService
    {
        GridOptions GetReport(GetCreditMovementCriteria criteria);

        GridOptions GetFullReportFromCache(GetCreditMovementCriteria criteria);

        string GetReportName(GetCreditMovementCriteria criteria);

        List<Site> GetSites();
    }
}
