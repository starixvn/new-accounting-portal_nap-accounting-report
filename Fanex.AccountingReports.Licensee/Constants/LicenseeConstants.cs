// --------------------------------------------------------------------------
// <copyright file="LicenseeConstants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.22">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Constants
{
    public enum ReportTimeType
    {
        /// <summary>
        /// All time type.
        /// </summary>
        All = 0,

        /// <summary>
        /// The daily type.
        /// </summary>
        Daily = 1,

        /// <summary>
        /// The monthly type.
        /// </summary>
        Monthly = 2
    }

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LicenseeConstants
    {
        public static readonly string LiveCasinoWinLossSessionKey = "Fanex.AccountingReports.Licensee.LiveCasinoWinLoss";
        public static readonly string NumberGameWinLossSessionKey = "Fanex.AccountingReports.Licensee.NumberGameWinLoss";
        public static readonly string PickConcurrentUserSessionKey = "Fanex.AccountingReports.Licensee.PickConcurrentUser";
        public static readonly string TotalTransationSessionKey = "Fanex.AccountingReports.Licensee.TotalTransactions.TotalTransactions";
        public static readonly string TotalTransactionResellerSessionKey = "Fanex.AccountingReports.Licensee.TotalTransactions.Reseller";
        public static readonly string NegativeBalanceSessionKey = "Fanex.AccountingReports.Licensee.NegativeBalance";
        public static readonly string LicenseeWithEnableBASessionKey = "Fanex.AccountingReports.Licensee.WinLossByLicensee.LicenseeWithEnableBA";
        public static readonly string VirtualSportsSessionKey = "Fanex.AccountingReports.Licensee.VirtualReports";

        public static readonly string WinLossByLicenseeCurrencyReportSessionKey = "Fanex.AccountingReports.Licensee.WinLossByLicensee.CurrencyReport";
        public static readonly string WinLossByLicenseeAgentReportSessionKey = "Fanex.AccountingReports.Licensee.WinLossByLicensee.AgentReport";
        public static readonly string WinLossByLicenseeMemberReportSessionKey = "Fanex.AccountingReports.Licensee.WinLossByLicensee.MemberReport";

        public static readonly string KenoWinLossReportSessionKey = "Fanex.AccountingReports.Licensee.KenoWinLossReport";
        public static readonly string AGCasinoWinLossMainReportSessionKey = "Fanex.AccountingReports.Licensee.AGCasinoWinLossMainReport";
        public static readonly string AGCasinoWinLossSuperReportSessionKey = "Fanex.AccountingReports.Licensee.AGCasinoWinLossSuperReport";
        public static readonly string AGCasinoWinLossMasterReportSessionKey = "Fanex.AccountingReports.Licensee.AGCasinoWinLossMasterReport";
        public static readonly string AGCasinoWinLossAgentReportSessionKey = "Fanex.AccountingReports.Licensee.AGCasinoWinLossAgentReport";
        public static readonly string AGCasinoWinLossMemberReportSessionKey = "Fanex.AccountingReports.Licensee.AGCasinoWinLossMemberReport";
    }
}
