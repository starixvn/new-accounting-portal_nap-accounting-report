// -----------------------------------------------------------------------
// <copyright file="IMetricGamingService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2h
{
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;

    /// <summary>
    /// Metric gaming service interface.
    /// </summary>
    public interface IMetricGamingService
    {
        LiveAndNoneLiveReport GetMetricGamingBetTypes(GetMetricGamingBettypeCriteria criteria);

        void ExportMetricGamingBettypeReport(GetMetricGamingBettypeCriteria criteria);
    }
}