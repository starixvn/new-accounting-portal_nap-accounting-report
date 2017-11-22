namespace Fanex.AccountingReports.WinLoss.Services.Interface.DailyMixParlayWinLoss
{
    using Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss;
    using Fanex.AccountingReports.WinLoss.Models.DailyMixParlayWinLoss;

    public interface IDailyMixParlayWinLossService
    {
        WinLossInfo GetCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria);

        WinLossInfo GetAdvanceMixParlayReport(GetAdvanceMixParlayReportCriteria criteria);

        void ExportCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria);

        void ExportAdvanceMixParlayReport(GetAdvanceMixParlayReportCriteria criteria);

        WinLossInfo GetMixParlayReport(GetMixParlayReportCriteria criteria);

        void ExportMixParlayReport(GetMixParlayReportCriteria criteria);
    }
}
