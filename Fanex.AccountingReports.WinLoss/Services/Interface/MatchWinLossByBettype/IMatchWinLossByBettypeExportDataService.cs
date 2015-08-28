// -----------------------------------------------------------------------
// <copyright file="IMatchWinLossByBettypeExportDataService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.16">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Services.Interface.MatchWinLossByBettype
{
    using Fanex.AccountingReports.WinLoss.Criteria.MatchWinLossByBettype;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IMatchWinLossByBettypeExportDataService
    {
        void ExportMatchWinLossByBettypeReport(MatchWinLossByBettypeCriteria getMatchWinLossByBettypeCriteria);
    }
}