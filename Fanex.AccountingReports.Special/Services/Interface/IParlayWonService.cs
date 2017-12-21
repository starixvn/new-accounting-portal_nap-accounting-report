namespace Fanex.AccountingReports.Special.Services.Interface
{
    using Fanex.AccountingReports.Special.Criteria.ParlayWon;
    using Fanex.AccountingReports.Special.Models.ParlayWon;

    public interface IParlayWonService
    {
        ParlayWonTransaction GetReport(GetParlayWonReportCriteria criteria);

        ParlayWonTransaction SortReport(GetParlayWonReportCriteria criteria);

        void ExportReport(GetParlayWonReportCriteria criteria);
    }
}