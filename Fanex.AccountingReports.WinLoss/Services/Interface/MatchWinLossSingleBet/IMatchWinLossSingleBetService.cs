// -----------------------------------------------------------------------
// <copyright file="IMatchWinLossSingleBetService.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Services.Interface.MatchWinLossSingleBet
{
    using Fanex.AccountingReports.WinLoss.Criteria.MatchWinLossSingleBet;
    using Fanex.AccountingReports.WinLoss.Models.MatchWinLossSingleBet;

    /// <summary>
    /// The interface of MatchWinLossSingleBetService.
    /// </summary>
    public interface IMatchWinLossSingleBetService
    {
        MatchWinLossSingleBet GetReport(GetMatchWinLossSingleBetCriteria criteria);

        void ExportReport(GetMatchWinLossSingleBetCriteria criteria);
    }
}
