namespace Fanex.AccountingReports.Gaming.Repositories.Interface.NumberGameWinLossByBetTypeMatch
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IWinLossByBetTypeRepository
    {
        List<WinLossInfo> GetMainReport(GetWinLossByBetTypeMainReportCriteria criteria);

        List<WinLossDetailInfo> GetDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria);
    }
}
