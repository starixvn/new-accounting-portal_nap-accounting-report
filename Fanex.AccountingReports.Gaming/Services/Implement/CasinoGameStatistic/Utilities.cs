// -----------------------------------------------------------------------
// <copyright file="CasinoGameStatisticUtilities.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

using Fanex.AccountingReports.Core.Extensions;
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
        public static string GetTitleExcelFileWhenDrilDown(GetReportByDailyCriteria criteria)
        {
            string title = string.Empty;
            switch (criteria.ReportType)
            {
                case PeriodTypeConstants.CasinoGameStatisticDaily:
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
            switch (criteria.ReportType)
            {
                case PeriodTypeConstants.CasinoGameStatisticMonthly:
                    var fromDate = Converter.To<DateTime>(Converter.ToDateTime(criteria.FromDate).ToString("yyyy-MM-01"));
                    var toDate = DateTimeExtensions.LastDayOfMonth(fromDate);
                    criteria.FromDate = fromDate.ToString();
                    criteria.ToDate = toDate.ToString();
                    break;
            }
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
                    name = dateTime.ToShortFormatedDate();
                    break;
                case PeriodTypeConstants.CasinoGameStatisticMonthly:
                    name = dateTime.ToMonthYearWithSlash();
                    break;
            }

            return name;
        }
    }
}