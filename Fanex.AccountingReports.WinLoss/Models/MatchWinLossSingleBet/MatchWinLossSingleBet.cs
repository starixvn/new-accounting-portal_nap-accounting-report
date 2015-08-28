// -----------------------------------------------------------------------
// <copyright file="MatchWinLossSingleBet.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Models.MatchWinLossSingleBet
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The model describes the report.
    /// </summary>
    [Serializable]
    public class MatchWinLossSingleBet
    {
        /// <summary>
        /// Gets or sets the name of home team.
        /// </summary>
        /// <value>The name of home team.</value>
        public string HomeName { get; set; }

        /// <summary>
        /// Gets or sets the name of away team.
        /// </summary>
        /// <value>The name of away team.</value>
        public string AwayName { get; set; }

        /// <summary>
        /// Gets or sets the win/lost of match.
        /// </summary>
        /// <value>The win/lost of match.</value>
        public decimal MatchWinLost { get; set; }

        public List<MatchWinLossSingleBet> Rows { get; set; }
    }
}
