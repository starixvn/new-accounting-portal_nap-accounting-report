// --------------------------------------------------------------------------
// <copyright file="MatchGameWinLossInfo.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.06.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Models.WinLossSummary3rd
{
    using System;
    using Fanex.AccountingReports.Core.Models;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    [Serializable]
    public class MatchGameWinLossInfo
    {
        public int MatchId { get; set; }

        // Is race column of Racing & Virtual Racing
        public string MatchCode { get; set; }

        public EntityBase<int, string> HomeTeam { get; set; }

        public EntityBase<int, string> AwayTeam { get; set; }

        // Is meetingid column of Racing
        public int LeagueId { get; set; }

        public WinLossInfo WinLossDetail { get; set; }

        public bool IsNeutral { get; set; }
    }
}
