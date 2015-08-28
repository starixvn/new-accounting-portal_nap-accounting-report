// -----------------------------------------------------------------------
// <copyright file="IDailyDetailChangedTurnoverService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Special.Services.Interface.DailyTurnover
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Special.Criteria.DailyTurnover;
    using Fanex.AccountingReports.Special.Models.DailyTurnover;

    /// <summary>
    /// The interface of daily detail changed turnover.
    /// </summary>
    public interface IDailyDetailChangedTurnoverService
    {
        List<DailyDetailChangedTurnover> GetDetailReport(GetDailyTurnoverChangedDetailCriteria criteria);
    }
}
