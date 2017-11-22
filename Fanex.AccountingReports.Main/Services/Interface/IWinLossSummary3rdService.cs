namespace Fanex.AccountingReports.Main.Services.Interface
{
    using Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd;
    using Fanex.AccountingReports.Main.Models.WinLossSummary3rd;
    using Fanex.Data.Repository;

    public interface IWinLossSummary3rdService
    {
        WinLoss3rdInfo GetReport(GetReportCriteria criteria);

        void ExportReport(GetReportCriteria criteria);

        void RecalculateReport(CriteriaBase criteria);

        MatchGame3rdInfo GetDetails(GetMatchGameDetailsCriteria criteria);

        void ExportDetails(GetMatchGameDetailsCriteria criteria);
    }
}
