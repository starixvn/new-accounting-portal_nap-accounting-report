namespace Fanex.AccountingReports.Gaming.Repositories.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ILiveCasinoGameStatisticRepository
    {
        List<List<WinLossInfoEntity>> GetReport(GetReportCriteria criteria);
    }
}
