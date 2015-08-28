// -----------------------------------------------------------------------
// <copyright file="MatchWinLossByBettype.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.13">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Models.MatchWinLossByBettype
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Match Win Loss By Bet Type Model.
    /// </summary>
    [Serializable]
    public class MatchWinLossByBettype
    {
        public MatchWinLossByBettype()
        {
            ReportRows = new List<MatchWinLossByBettype>();
        }

        public bool IsLive { get; set; }

        public string Bettype { get; set; }

        public string HomeTeam { get; set; }

        public string AwayTeam { get; set; }

        public decimal WinLoss { get; set; }

        public decimal TotalWinLoss { get; set; }

        public List<MatchWinLossByBettype> ReportRows { get; set; }

        public void CalculateTotal(MatchWinLossByBettype reportInfo)
        {
            this.TotalWinLoss += reportInfo.WinLoss;
        }
    }
}