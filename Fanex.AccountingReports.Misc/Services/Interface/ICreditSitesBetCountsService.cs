namespace Fanex.AccountingReports.Misc.Services.Interface
{
    using Fanex.AccountingReports.Misc.Criteria.CreditSitesBetCounts;
    using Fanex.AccountingReports.Misc.Models;

    public interface ICreditSitesBetCountsService
    {
        Transaction GetReport(GetCreditSitesBetCountsCriteria criteria);

        void ExportReport(GetCreditSitesBetCountsCriteria criteria);
    }
}
