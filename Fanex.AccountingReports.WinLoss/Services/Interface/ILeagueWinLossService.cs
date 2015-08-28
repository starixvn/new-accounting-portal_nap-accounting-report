// --------------------------------------------------------------------------
// <copyright file="ILeagueWinLossService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Services.Interface
{
    using Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss;
    using Fanex.AccountingReports.WinLoss.Models.LeagueWinLoss;

    public interface ILeagueWinLossService
    {
        LeagueWinLossInfo GetLeagueWinLoss(GetLeagueWinLossCriteria criteria);

        LeagueWinLossInfo GetLeagueWinLossBySport(GetLeagueWinLossBySportCriteria criteria);

        LeagueWinLossInfo GetLeagueWinLossByBetType(GetLeagueWinLossByBetTypeCriteria criteria);

        void ExportLeagueWinLoss(GetLeagueWinLossCriteria criteria);

        void ExportLeagueWinLossBySport(GetLeagueWinLossBySportCriteria criteria);

        void ExportLeagueWinLossByBetType(GetLeagueWinLossByBetTypeCriteria criteria);
    }
}
