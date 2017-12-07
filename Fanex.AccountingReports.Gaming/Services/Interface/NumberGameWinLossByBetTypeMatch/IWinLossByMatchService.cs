namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameWinLossByBetTypeMatch
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch;

    public interface IWinLossByMatchService
    {
        WinLossInfo GetMainReport(GetWinLossByMatchMainReportCriteria criteria);

        WinLossInfo GetDetailReport(GetWinLossByMatchDetailReportCriteria criteria);

        void ExportMainReport(GetWinLossByMatchMainReportCriteria criteria);

        void ExportDetailReport(GetWinLossByMatchDetailReportCriteria criteria);
    }
}
