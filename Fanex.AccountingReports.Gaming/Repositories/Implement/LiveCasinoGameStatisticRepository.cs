// --------------------------------------------------------------------------
// <copyright file="LiveCasinoGameStatisticRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoGameStatisticRepository : ILiveCasinoGameStatisticRepository
    {
        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<List<WinLossInfoEntity>> GetReport(GetReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<List<WinLossInfoEntity>> report = new List<List<WinLossInfoEntity>>();
            if (result == null)
            {
                return report;
            }

            if (result.Tables.Count >= 1)
            {
                var winLossInfoTable = result.Tables[0];
                var winLossInfo = new List<WinLossInfoEntity>();
                foreach (DataRow row in winLossInfoTable.Rows)
                {
                    var info = new WinLossInfoEntity
                    {
                        Id = GetWinLossInfoId(criteria, row),
                        NumberOfPlayers = criteria.ReportType == PeriodTypeConstants.LiveCasinoGameStatisticUsername ?
                            0 :
                            Converter.ToInt(row["NoOfPlayers"]),
                        NumberOfGames = Converter.ToInt(row["NoOfGames"]),
                        CurrencyName = Converter.ToString2(row["Currency"]),

                        TotalBets = Converter.ToDecimal(row["TotalBets"]),
                        TotalWins = Converter.ToDecimal(row["TotalWins"]),
                        GameIncome = Converter.ToDecimal(row["GameIncome"]),
                        GamePayout = Converter.ToDecimal(row["GamePayout"]),
                        TotalRefund = Converter.ToDecimal(row["TotalRefund"]),
                        CompanyNetIncome = Converter.ToDecimal(row["CompanyNetIncome"]),
                        CompanyTurnover = Converter.ToDecimal(row["CompanyTurnOver"]),
                        CompanyCommission = Converter.ToDecimal(row["CompanyComm"]),

                        TotalBetsRM = Converter.ToDecimal(row["TotalBets_RM"]),
                        TotalWinsRM = Converter.ToDecimal(row["TotalWins_RM"]),
                        GameIncomeRM = Converter.ToDecimal(row["GameIncome_RM"]),
                        GamePayoutRM = Converter.ToDecimal(row["GamePayout_RM"]),
                        TotalRefundRM = Converter.ToDecimal(row["TotalRefund_RM"]),
                        CompanyNetIncomeRM = Converter.ToDecimal(row["CompanyNetIncome_RM"]),
                        CompanyTurnoverRM = Converter.ToDecimal(row["CompanyTurnOver_RM"]),
                        CompanyCommissionRM = Converter.ToDecimal(row["CompanyComm_RM"])
                    };

                    winLossInfo.Add(info);
                }

                report.Add(winLossInfo);
            }

            if (result.Tables.Count >= 2)
            {
                var grandTotalInfoTable = result.Tables[1];
                var grandTotalInfo = new List<WinLossInfoEntity>();
                foreach (DataRow row in grandTotalInfoTable.Rows)
                {
                    grandTotalInfo.Add(new WinLossInfoEntity
                    {
                        NumberOfPlayers = criteria.ReportType == PeriodTypeConstants.LiveCasinoGameStatisticUsername ?
                            0 :
                            Converter.ToInt(row["NoOfPlayers"]),
                        NumberOfGames = Converter.ToInt(row["NoOfGames"]),
                        CurrencyName = Converter.ToString2(row["Currency"]),

                        TotalBets = Converter.ToDecimal(row["TotalBets"]),
                        TotalWins = Converter.ToDecimal(row["TotalWins"]),
                        GameIncome = Converter.ToDecimal(row["GameIncome"]),
                        GamePayout = Converter.ToDecimal(row["GamePayout"]),
                        TotalRefund = Converter.ToDecimal(row["TotalRefund"]),
                        CompanyNetIncome = Converter.ToDecimal(row["CompanyNetIncome"]),
                        CompanyTurnover = Converter.ToDecimal(row["CompanyTurnOver"]),
                        CompanyCommission = Converter.ToDecimal(row["CompanyComm"])
                    });
                }

                report.Add(grandTotalInfo);
            }

            return report;
        }

        /// <summary>
        /// Gets the win loss information identifier.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns>The win loss information identifier.</returns>
        private string GetWinLossInfoId(GetReportCriteria criteria, DataRow dataRow)
        {
            switch (criteria.ReportType)
            {
                case PeriodTypeConstants.LiveCasinoGameStatisticDaily:
                    return Converter.ToDateTime(dataRow["Date"]).ToShortFormatedDate();
                case PeriodTypeConstants.LiveCasinoGameStatisticMonthly:
                    return string.Format("{0}-{1}", dataRow["Year"], dataRow["Month"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticUsername:
                    return Converter.ToString2(dataRow["Username"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticGameType:
                    return Converter.ToString2(dataRow["GameTypeID"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticGameTable:
                    return Converter.ToString2(dataRow["GameTableID"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticCompany:
                    return Converter.ToString2(dataRow["Company"]);
                case PeriodTypeConstants.LiveCasinoGameStatisticCurrency:
                    return Converter.ToString2(dataRow["Currency"]);
                default:
                    return string.Empty;
            }
        }
    }
}
