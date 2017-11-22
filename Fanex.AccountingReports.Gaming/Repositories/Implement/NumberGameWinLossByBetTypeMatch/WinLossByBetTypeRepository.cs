namespace Fanex.AccountingReports.Gaming.Repositories.Implement.NumberGameWinLossByBetTypeMatch
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Repositories.Interface.NumberGameWinLossByBetTypeMatch;
    using Fanex.Common;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByBetTypeRepository : IWinLossByBetTypeRepository
    {
        /// <summary>
        /// Gets the main report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The main report.</returns>
        public List<WinLossInfo> GetMainReport(GetWinLossByBetTypeMainReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                if (criteria.LiveStatus == -1)
                {
                    criteria.LiveStatus = null;
                }

                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<WinLossInfo> report = new List<WinLossInfo>();
            if (result == null || result.Tables.Count == 0)
            {
                return report;
            }

            var winLossInfoTable = result.Tables[0];
            foreach (DataRow row in winLossInfoTable.Rows)
            {
                var info = new WinLossInfo
                {
                    Id = Converter.ToInt(row["bettype"]),
                    CustomerWinLoss = Converter.ToDecimal(row["WinLost"]),
                    AgencyWinLoss = Converter.ToDecimal(row["Position"]),
                    Commission = Converter.ToDecimal(row["Commission"]),
                    LineTotal = Converter.ToDecimal(row["LineTotal"]),
                    VirtualLineTotal = Converter.ToDecimal(row["VirtualLineTotal"]),
                    Turnover = Converter.ToDecimal(row["Turnover"])
                };

                report.Add(info);
            }

            return report;
        }

        /// <summary>
        /// Gets the detail report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The detail report.</returns>
        public List<WinLossDetailInfo> GetDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                if (criteria.LiveStatus == -1)
                {
                    criteria.LiveStatus = null;
                }

                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<WinLossDetailInfo> report = new List<WinLossDetailInfo>();
            if (result == null || result.Tables.Count == 0)
            {
                return report;
            }

            var winLossInfoTable = result.Tables[0];
            foreach (DataRow row in winLossInfoTable.Rows)
            {
                var info = new WinLossDetailInfo
                {
                    CustomerWinLoss = Converter.ToDecimal(row["WinLost"]),
                    AgencyWinLoss = Converter.ToDecimal(row["Position"]),
                    Commission = Converter.ToDecimal(row["Commission"]),
                    LineTotal = Converter.ToDecimal(row["LineTotal"]),
                    VirtualLineTotal = Converter.ToDecimal(row["VirtualLineTotal"]),
                    Turnover = Converter.ToDecimal(row["Turnover"]),
                    MatchCode = Converter.ToString2(row["matchcode"]),
                    BetType = Converter.ToInt(row["bType"]),
                    LeagueId = Converter.ToInt(row["leagueid"])
                };

                report.Add(info);
            }

            return report;
        }
    }
}
