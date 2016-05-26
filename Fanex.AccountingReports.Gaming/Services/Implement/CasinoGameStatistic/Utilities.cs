// -----------------------------------------------------------------------
// <copyright file="CasinoGameStatisticUtilities.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Services.Implement.CasinoGameStatistic
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Resources;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Utilities
    {
        private static string _reportDailyKey = "Gaming.CasinoGameStatistic.Daily." + AppSession.CurrentUser.Id;
        private static string _reportDailyByCurrencyKey = "Gaming.CasinoGameStatistic.DailyByCurrency." + AppSession.CurrentUser.Id;
        private static string _reportMonthlyKey = "Gaming.CasinoGameStatistic.Monthly." + AppSession.CurrentUser.Id;
        private static string _reportMonthlyByCurrencyKey = "Gaming.CasinoGameStatistic.MonthlyByCurrency." + AppSession.CurrentUser.Id;
        private static string _reportUserNameKey = "Gaming.CasinoGameStatistic.UserName." + AppSession.CurrentUser.Id;
        private static string _reportDailyByUserNameKey = "Gaming.CasinoGameStatistic.DailyByUserName." + AppSession.CurrentUser.Id;
        private static string _reportGameTypeKey = "Gaming.CasinoGameStatistic.GameType." + AppSession.CurrentUser.Id;
        private static string _reportGameTypeByCurrencyKey = "Gaming.CasinoGameStatistic.GameTypeByCurrency." + AppSession.CurrentUser.Id;
        private static string _reportGameTableKey = "Gaming.CasinoGameStatistic.GameTable." + AppSession.CurrentUser.Id;
        private static string _reportGameTableByCurrencyKey = "Gaming.CasinoGameStatistic.GameTableByCurrency." + AppSession.CurrentUser.Id;
        private static string _reportCompanyKey = "Gaming.CasinoGameStatistic.Company." + AppSession.CurrentUser.Id;
        private static string _reportCompanyByCurrencyKey = "Gaming.CasinoGameStatistic.CompanyByCurrency." + AppSession.CurrentUser.Id;
        private static string _reportCurrencyKey = "Gaming.CasinoGameStatistic.Currency." + AppSession.CurrentUser.Id;
        private static string _reportGrandTotalKey = "Gaming.CasinoGameStatistic.GrandTotal." + AppSession.CurrentUser.Id;
        private static string _reportHourlyKey = "Gaming.CasinoGameStatistic.Hourly." + AppSession.CurrentUser.Id;

        public enum ViewReport
        {
            Daily = 0,
            DailyByCurrency = 1,
            Monthly = 3,
            MonthlyByCurrency = 4,
            UserName = 5,
            DailyByUserName = 6,
            GameType = 7,
            GameTypeByCurrency = 8,
            GameTable = 9,
            GameTableByCurrency = 10,
            Company = 11,
            CompanyByCurrency = 12,
            Currency = 13,
            GrandTotal = 14,
            Hourly = 15
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public static string GetTitleExcelFileWhenDrilDown(GetReportByDailyCriteria criteria)
        {
            string title = string.Empty;
            switch (criteria.ReportType)
            {
                case PeriodTypeConstants.CasinoGameStatisticDaily:
                case PeriodTypeConstants.CasinoGameStatisticUsernameDaily:
                    title = string.Format(
                            GamingResources.CasinoGameStatisticOnDateByCurrency_ReportTitle,
                            criteria.Product,
                            Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate());
                    break;

                case PeriodTypeConstants.CasinoGameStatisticMonthly:
                    title = string.Format(
                            GamingResources.CasinoGameStatistic_MonthlyReportTitle,
                            criteria.Product,
                            Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                            Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());
                    break;

                case PeriodTypeConstants.CasinoGameStatisticUsername:
                    title = string.Format(
                            GamingResources.CasinoGameStatistic_UserName_ReportTitle,
                            criteria.Product,
                            Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                            Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());
                    break;

                case PeriodTypeConstants.CasinoGameStatisticGameType:
                    title = string.Format(
                            GamingResources.CasinoGameStatistic_GameType_ReportTitle,
                            criteria.Product,
                            Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                            Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());
                    break;

                case PeriodTypeConstants.CasinoGameStatisticGameTable:
                    title = string.Format(
                            GamingResources.CasinoGameStatistic_GameTable_ReportTitle,
                            criteria.Product,
                            Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                            Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());
                    break;

                case PeriodTypeConstants.CasinoGameStatisticCompany:
                    title = string.Format(
                            GamingResources.CasinoGameStatistic_Company_ReportTitle,
                            criteria.Product,
                            Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                            Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());
                    break;

                case PeriodTypeConstants.CasinoGameStatisticCurrency:
                    title = string.Format(
                            GamingResources.CasinoGameStatistic_Currency_ReportTitle,
                            criteria.Product,
                            Converter.ToDateTime(criteria.FromDate).ToShortFormatedDate(),
                            Converter.ToDateTime(criteria.ToDate).ToShortFormatedDate());
                    break;
            }

            return title;
        }

        public static void ReSetPeriodTimeByReportType(GetReportByDailyCriteria criteria)
        {
            switch (criteria.ViewReport)
            {
                case (int)ViewReport.MonthlyByCurrency:
                    ResetDayInMonth(criteria);
                    break;
            }
        }

        public static void ResetDayInMonth(GetReportByDailyCriteria criteria)
        {
            var fromDate = Converter.To<DateTime>(Converter.ToDateTime(criteria.FromDate).ToString("yyyy-MM-01"));
            var toDate = DateTimeExtensions.LastDayOfMonth(fromDate);
            criteria.FromDate = fromDate.ToString();
            criteria.ToDate = toDate.ToString();
        }

        public static void AssignGrandTotal(WinLossInfoEntity grandTotal, WinLossInfo grandTotalRM)
        {
            grandTotalRM.NumberOfPlayers = grandTotal.NumberOfPlayers;
            grandTotalRM.NumberOfGames = grandTotal.NumberOfGames;

            grandTotalRM.TotalBets = grandTotal.TotalBets;
            grandTotalRM.TotalWins = grandTotal.TotalWins;
            grandTotalRM.TotalRefund = grandTotal.TotalRefund;

            grandTotalRM.GameIncome = grandTotal.GameIncome;
            grandTotalRM.GamePayout = grandTotal.GamePayout;

            grandTotalRM.CompanyTurnover = grandTotal.CompanyTurnover;
            grandTotalRM.CompanyNetIncome = grandTotal.CompanyNetIncome;
            grandTotalRM.CompanyCommission = grandTotal.CompanyCommission;
        }

        public static string GetNameByReportType(int reportType, DateTime dateTime)
        {
            string name = string.Empty;
            switch (reportType)
            {
                case PeriodTypeConstants.CasinoGameStatisticDaily:
                case PeriodTypeConstants.CasinoGameStatisticUsernameDaily:
                case PeriodTypeConstants.CasinoGameStatisticMonthlyToDaily:
                    name = dateTime.ToShortFormatedDate();
                    break;

                case PeriodTypeConstants.CasinoGameStatisticMonthly:
                    name = dateTime.ToMonthYearWithSlash();
                    break;
            }

            return name;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]

        public static string GetReportKey(int viewReport)
        {
            string key = string.Empty;
            switch (viewReport)
            {
                case (int)ViewReport.Daily:
                    key = _reportDailyKey;
                    break;

                case (int)ViewReport.DailyByCurrency:
                    key = _reportDailyByCurrencyKey;
                    break;

                case (int)ViewReport.Monthly:
                    key = _reportMonthlyKey;
                    break;

                case (int)ViewReport.MonthlyByCurrency:
                    key = _reportMonthlyByCurrencyKey;
                    break;

                case (int)ViewReport.UserName:
                    key = _reportUserNameKey;
                    break;

                case (int)ViewReport.DailyByUserName:
                    key = _reportDailyByUserNameKey;
                    break;

                case (int)ViewReport.GameType:
                    key = _reportGameTypeKey;
                    break;

                case (int)ViewReport.GameTypeByCurrency:
                    key = _reportGameTypeByCurrencyKey;
                    break;

                case (int)ViewReport.GameTable:
                    key = _reportGameTableKey;
                    break;

                case (int)ViewReport.GameTableByCurrency:
                    key = _reportGameTableByCurrencyKey;
                    break;

                case (int)ViewReport.Company:
                    key = _reportCompanyKey;
                    break;

                case (int)ViewReport.CompanyByCurrency:
                    key = _reportCompanyByCurrencyKey;
                    break;

                case (int)ViewReport.Currency:
                    key = _reportCurrencyKey;
                    break;

                case (int)ViewReport.GrandTotal:
                    key = _reportGrandTotalKey;
                    break;

                case (int)ViewReport.Hourly:
                    key = _reportHourlyKey;
                    break;
            }

            return key;
        }
    }
}