namespace Fanex.AccountingReports.Gaming.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Extensions;
    using Fanex.AccountingReports.Gaming.Constants;
    using Fanex.AccountingReports.Gaming.Criteria.LiveCasinoWinLossByAgency;
    using Fanex.AccountingReports.Gaming.Models.LiveCasinoWinLossByAgency;
    using Fanex.AccountingReports.Gaming.Repositories.Interface;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LiveCasinoAgencyWinLossRepository : ILiveCasinoAgencyWinLossRepository
    {
        /// <summary>
        /// Gets the report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The report.</returns>
        [SuppressMessage("StyleCop.CSharp.StylecopPlus.Rules",
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
            if (result == null || result.Tables.Count < 3)
            {
                return report;
            }

            var winLossInfoTable = result.Tables[0];
            var winLossInfo = new List<WinLossInfoEntity>();
            foreach (DataRow row in winLossInfoTable.Rows)
            {
                var info = new WinLossInfoEntity
                {
                    Id = GetWinLossInfoId(criteria, row),
                    CurrencyName = Converter.ToString2(row["Currency"]),

                    RollingTurnover = Converter.ToDecimal(row["RollingTO"]),
                    CompanyTurnover = Converter.ToDecimal(row["CompanyTO"]),
                    GrossCommission = Converter.ToDecimal(row["GrossComm"]),
                    TotalBets = Converter.ToDecimal(row["Bets"]),

                    MemberCommission = Converter.ToDecimal(row["PlayerComm"]),
                    MemberWinLoss = Converter.ToDecimal(row["Winloss"]),

                    AgentCommission = Converter.ToDecimal(row["Comm"]),
                    AgentWinLoss = Converter.ToDecimal(row["AWinloss"]),

                    MasterCommission = Converter.ToDecimal(row["AComm"]),
                    MasterWinLoss = Converter.ToDecimal(row["MWinloss"]),

                    SuperCommission = Converter.ToDecimal(row["SComm"]),
                    SuperWinLoss = Converter.ToDecimal(row["SWinloss"]),

                    CompanyWinLoss = Converter.ToDecimal(row["CompanyWinLoss"])
                };

                winLossInfo.Add(info);
            }

            report.Add(winLossInfo);

            var grandTotalInfoTable = result.Tables[1];
            var grandTotalInfo = new List<WinLossInfoEntity>();
            foreach (DataRow row in grandTotalInfoTable.Rows)
            {
                grandTotalInfo.Add(new WinLossInfoEntity
                {
                    CurrencyName = Converter.ToString2(row["Currency"]),

                    RollingTurnover = Converter.ToDecimal(row["RollingTO"]),
                    CompanyTurnover = Converter.ToDecimal(row["CompanyTO"]),
                    GrossCommission = Converter.ToDecimal(row["GrossComm"]),
                    TotalBets = Converter.ToDecimal(row["Bets"]),

                    MemberCommission = Converter.ToDecimal(row["PlayerComm"]),
                    MemberWinLoss = Converter.ToDecimal(row["Winloss"]),

                    AgentCommission = Converter.ToDecimal(row["Comm"]),
                    AgentWinLoss = Converter.ToDecimal(row["AWinloss"]),

                    MasterCommission = Converter.ToDecimal(row["AComm"]),
                    MasterWinLoss = Converter.ToDecimal(row["MWinloss"]),

                    SuperCommission = Converter.ToDecimal(row["SComm"]),
                    SuperWinLoss = Converter.ToDecimal(row["SWinloss"]),

                    CompanyWinLoss = Converter.ToDecimal(row["CompanyWinLoss"])
                });
            }

            report.Add(grandTotalInfo);

            var roleInfoTable = result.Tables[2];
            var roleInfo = Converter.ToInt(roleInfoTable.Rows[0]["ShowDetail"]);
            report.Add(roleInfo);
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
                case PeriodTypeConstants.LiveCasinoAgencyWinLossDaily:
                    return Converter.ToDateTime(dataRow["Date"]).ToShortFormatedDate();
                case PeriodTypeConstants.LiveCasinoAgencyWinLossMonthly:
                    return Converter.ToDateTime(dataRow["Month"]).ToString("yyyy/MM");
                case PeriodTypeConstants.LiveCasinoAgencyWinLossUsername:
                    return Converter.ToString2(dataRow["username"]);
                case PeriodTypeConstants.LiveCasinoAgencyWinLossGameType:
                    return Converter.ToString2(dataRow["GameType"]);
                case PeriodTypeConstants.LiveCasinoAgencyWinLossGameTable:
                    return Converter.ToString2(dataRow["GameTable"]);
                case PeriodTypeConstants.LiveCasinoAgencyWinLossAgent:
                    return Converter.ToString2(dataRow["username"]);
                default:
                    return string.Empty;
            }
        }
    }
}
