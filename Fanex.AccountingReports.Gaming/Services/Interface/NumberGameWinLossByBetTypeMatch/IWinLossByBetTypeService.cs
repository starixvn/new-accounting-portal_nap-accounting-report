namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameWinLossByBetTypeMatch
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch;

    public interface IWinLossByBetTypeService
    {
        WinLossInfo GetMainReport(GetWinLossByBetTypeMainReportCriteria criteria);

        WinLossInfo GetDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria);

        void ExportMainReport(GetWinLossByBetTypeMainReportCriteria criteria);

        void ExportDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria);
    }
}
