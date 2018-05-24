namespace Fanex.AccountingReports.Special.Services.Interface.IDailyTurnover
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Special.Criteria.DailyTurnover;
    using Fanex.AccountingReports.Special.Models.DailyTurnover;

    /// <summary>
    /// The interface of Daily Turnover Service.
    /// </summary>
    public interface IDailyTurnoverService
    {
        List<DailyTurnover> GetReport(GetDailyTurnoverCriteria criteria);

        void ExportReport(GetDailyTurnoverCriteria criteria);
    }
}
