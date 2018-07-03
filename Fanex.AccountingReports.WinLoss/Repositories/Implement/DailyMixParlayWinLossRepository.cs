namespace Fanex.AccountingReports.WinLoss.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss;
    using Fanex.AccountingReports.WinLoss.Repositories.Interface;
    using Fanex.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyMixParlayWinLossRepository : IDailyMixParlayWinLossRepository
    {
        /// <summary>
        /// Gets the combination win loss report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns>The combination win loss report.</returns>
        public List<dynamic> GetCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria.GetSpParams());
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        LiveType = row["LiveType"],
                        Status = row["status"],
                        WinLost = row["WinLost"],
                        TurnoverP = row["TurnoverP"],
                        BetCount = row["BetCount"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        LineTotal = row["LineTotal"],
                        Turnover = row["Turnover"],
                        Combinations = row["Combinations"],
                        ExtraPT = row["ExtraPT"]
                    });
                }
            }

            return report;
        }
    }
}
