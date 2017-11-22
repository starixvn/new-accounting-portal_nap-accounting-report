namespace Fanex.AccountingReports.Racing.Services.Interface.RacingGroupDaily
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Racing.Criteria.RacingGroupDaily;
    using Fanex.AccountingReports.Racing.Models.RacingGroupDaily;

    /// <summary>
    /// The interface of Racing Group Daily report.
    /// </summary>
    public interface IRacingGroupDailyGetReportServices
    {
        List<RacingGroupDaily> GetReport(GetWinLostRacingGroupCriteria criteria);
    }
}
