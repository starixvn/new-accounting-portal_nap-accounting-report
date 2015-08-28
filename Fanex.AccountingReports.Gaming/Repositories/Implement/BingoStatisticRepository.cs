// --------------------------------------------------------------------------
// <copyright file="BingoStatisticRepository.cs" company="Nexcel Solutions Vietnam">
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
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.BingoStatistic;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BingoStatisticRepository : IBingoStatisticRepository
    {
        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
            "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public List<dynamic> GetReport(GetReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null)
            {
                if (result.Tables.Count >= 1)
                {
                    var winLossInfoTable = result.Tables[0];
                    var winLossInfo = new List<dynamic>();
                    foreach (DataRow row in winLossInfoTable.Rows)
                    {
                        var info = new
                        {
                            Id = this.GetWinLossInfoId(criteria, row),
                            NoOfPlayers = criteria.ReportType == PeriodTypeConstants.GamingBingoStatisticUsername ?
                                0 : row["NoOfPlayers"],
                            Currency = row["Currency"],
                            TotalBets = row["Total Bets"],
                            TotalWins = row["Total Wins"],
                            GameIncome = row["Game Income"],
                            GamePayout = row["Game Payout"],
                            CompanyNetIncome = row["Company Net Income"],
                            CompanyComm = row["Company Comm"]
                        };

                        winLossInfo.Add(info);
                    }

                    report.Add(winLossInfo);
                }

                if (result.Tables.Count >= 2)
                {
                    var grandTotalInfoTable = result.Tables[1];
                    var grandTotalInfo = new List<dynamic>();
                    foreach (DataRow row in grandTotalInfoTable.Rows)
                    {
                        grandTotalInfo.Add(new
                        {
                            Id = row["Currency"],
                            NoOfPlayers = criteria.ReportType == PeriodTypeConstants.GamingBingoStatisticUsername ?
                                0 : row["NoOfPlayers"],
                            Currency = row["Currency"],
                            TotalBets = row["Total Bets"],
                            TotalWins = row["Total Wins"],
                            GameIncome = row["Game Income"],
                            GamePayout = row["Game Payout"],
                            CompanyNetIncome = row["Company Net Income"],
                            CompanyComm = row["Company Comm"]
                        });
                    }

                    report.Add(grandTotalInfo);
                }
            }

            return report;
        }

        /// <summary>
        /// Gets the win loss information identifier.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="dataRow">The data row.</param>
        /// <returns>The win loss information identifier.</returns>
        private object GetWinLossInfoId(GetReportCriteria criteria, DataRow dataRow)
        {
            switch (criteria.ReportType)
            {
                case PeriodTypeConstants.GamingBingoStatisticDaily:
                    return dataRow["Date"];
                case PeriodTypeConstants.GamingBingoStatisticMonthly:
                    return string.Format("{0}-{1}", dataRow["Year"], dataRow["Month"]);
                case PeriodTypeConstants.GamingBingoStatisticUsername:
                    return dataRow["Username"];
                case PeriodTypeConstants.GamingBingoStatisticCompany:
                    return dataRow["Company"];
                case PeriodTypeConstants.GamingBingoStatisticCurrency:
                    return dataRow["Currency"];
                case PeriodTypeConstants.GamingBingoStatisticAgent:
                    return dataRow["Username"];
                default:
                    return string.Empty;
            }
        }
    }
}
