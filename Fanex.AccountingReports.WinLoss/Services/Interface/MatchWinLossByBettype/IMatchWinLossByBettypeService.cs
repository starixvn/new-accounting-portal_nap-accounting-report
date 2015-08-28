// -----------------------------------------------------------------------
// <copyright file="IMatchWinLossByBettypeService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.13">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Services.Interface.MatchWinLossByBettype
{
    using Fanex.AccountingReports.WinLoss.Criteria.MatchWinLossByBettype;
    using Fanex.AccountingReports.WinLoss.Models.MatchWinLossByBettype;

    public interface IMatchWinLossByBettypeService
    {
        MatchWinLossByBettype GetMatchWinLossByBettype(MatchWinLossByBettypeCriteria getMatchWinLossByBettypeCriteria);
    }
}