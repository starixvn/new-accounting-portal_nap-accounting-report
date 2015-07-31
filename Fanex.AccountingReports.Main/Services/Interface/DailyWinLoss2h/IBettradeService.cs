// -----------------------------------------------------------------------
// <copyright file="IBettradeService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2h
{
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IBettradeService
    {
        WinLossInfo GetBettradeReport(GetBettradeCriteria criteria);

        void ExportBettradeReport(GetBettradeCriteria criteria);
    }
}