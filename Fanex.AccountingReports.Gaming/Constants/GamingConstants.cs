// --------------------------------------------------------------------------
// <copyright file="GamingConstants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Constants
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class GamingConstants
    {
        /// <summary>
        /// Number Game Win Los sBy Date SessionKey.
        /// </summary>
        public static readonly string NumberGameWinLossByDateSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByDate";

        /// <summary>
        /// Number Game Win Loss By Currency Main SessionKey.
        /// </summary>
        public static readonly string NumberGameWinLossByCurrencyMainSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByCurrencyMain";

        /// <summary>
        /// Number Game Win Loss By Currency Super SessionKey.
        /// </summary>
        public static readonly string NumberGameWinLossByCurrencySuperSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByCurrencySuper";

        /// <summary>
        /// Number Game Win Loss By Currency Master SessionKey.
        /// </summary>
        public static readonly string NumberGameWinLossByCurrencyMasterSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByCurrencyMaster";

        /// <summary>
        /// Number Game Win Loss By Currency Agent SessionKey.
        /// </summary>
        public static readonly string NumberGameWinLossByCurrencyAgentSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByCurrencyAgent";

        /// <summary>
        /// Number Game Win Loss By Currency Member SessionKey.
        /// </summary>
        public static readonly string NumberGameWinLossByCUrrencyMemberSessionKey = "Fanex.AccountingReports.Gaming.NumberGameWinlossByCurrencyMember";        

        /// <summary>
        /// Bingo Win Loss Report SessionKey.
        /// </summary>
        public static readonly string BingoWinLossReportSessionKey = "Fanex.AccountingReports.Gaming.BingoWinLoss.Report";

        /// <summary>
        /// Live Casino Game Financial Report SessionKey.
        /// </summary>
        public static readonly string LiveCasinoGameFinancialReportSessionKey = "Fanex.AccountingReports.Gaming.LiveCasinoGames.FinancialReport";

        /// <summary>
        /// Bingo Statistic Report SessionKey.
        /// </summary>
        public static readonly string BingoStatisticReportSessionKey = "Fanex.AccountingReports.Gaming.BingoStatistic.Report";

        /// <summary>
        /// Bingo Outstanding Report SessionKey.
        /// </summary>
        public static readonly string BingoOutstandingReportSessionKey = "Fanex.AccountingReports.Gaming.BingoOutStanding.Report";

        public static readonly int MaxRowsData = 65000;
        
        public enum ReportRowType
        {
            /// <summary>
            /// The total.
            /// </summary>
            Total = 1,

            /// <summary>
            /// The GrandTotal.
            /// </summary>
            GrandTotal = 2,

            /// <summary>
            /// The HeaderSite.
            /// </summary>
            HeaderSite = 3,

            /// <summary>
            /// The EvenRow.
            /// </summary>
            EvenRow = 4,

            /// <summary>
            /// The OddRow.
            /// </summary>
            OddRow = 5
        }
    }
}
