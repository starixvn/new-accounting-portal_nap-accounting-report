// -----------------------------------------------------------------------
// <copyright file="CasinoGameStatisticUtilities.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Fanex.AccountingReports.Core.Extensions;
using Fanex.AccountingReports.Core.Web;
using Fanex.AccountingReports.Gaming.Constants;
using Fanex.AccountingReports.Gaming.Resources;

namespace Fanex.AccountingReports.Gaming.Services.Implement.CasinoGameStatistic
{
    using System;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;
    using Fanex.Common;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class Utilities
    {
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
            CompanyByCurrency = 12
        }

        private static string _reportDailyKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.Daily." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportDailyByCurrencyKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.DailyByCurrency." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportMonthlyKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.Monthly." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportMonthlyByCurrencyKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.MonthlyByCurrency." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportUserNameKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.UserName." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportDailyByUserNameKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.DailyByUserName." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportGameTypeKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.GameType." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportGameTypeByCurrencyKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.GameTypeByCurrency." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportGameTableKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.GameTable." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportGameTableByCurrencyKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.GameTableByCurrency." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportCompanyKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.Company." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);
        private static string _reportCompanyByCurrencyKey = "Fanex.AccountingReports.Gaming.CasinoGameStatistic.CompanyByCurrency." + (AppSession.CurrentUser != null ? AppSession.CurrentUser.Id : 0);

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
            grandTotalRM.Sum(new WinLossInfo
            {
                NumberOfPlayers = grandTotal.NumberOfPlayers,
                NumberOfGames = grandTotal.NumberOfGames,

                TotalBets = grandTotal.TotalBets,
                TotalWins = grandTotal.TotalWins,
                TotalRefund = grandTotal.TotalRefund,

                GameIncome = grandTotal.GameIncome,

                CompanyTurnover = grandTotal.CompanyTurnover,
                CompanyNetIncome = grandTotal.CompanyNetIncome,
                CompanyCommission = grandTotal.CompanyCommission
            });
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
            }

            return key;
        }
    }
}