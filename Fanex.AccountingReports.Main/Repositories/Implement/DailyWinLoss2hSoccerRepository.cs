namespace Fanex.AccountingReports.Main.Repositories.Implement
{
    using Core.Services.Impl;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccer;
    using Fanex.AccountingReports.Main.Repositories.Interface;
    using Fanex.Data;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class DailyWinLoss2hSoccerRepository : IDailyWinLoss2hSoccerRepository
    {
        /// <summary>
        /// Gets the main2h report.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output parameter.</param>
        /// <returns>The main2h report.</returns>
        public List<dynamic> GetMain2hReport(
            GetMain2hReportCriteria criteria,
            ref Hashtable outputParam)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = new ObjectDb(settingKey))
            {
                Hashtable param = criteria.GetSpParams();

                if (ReportService.IsRequestDataWH(criteria.Winlostdate))
                {
                    param.Add("linetotal", 0);
                }

                result = objectDb.ExecuteDataSet(param);
                outputParam = objectDb.GetOuputParams();
            }

            List<dynamic> report = new List<dynamic>();
            if (result != null && result.Tables.Count >= 1)
            {
                var dataTable = result.Tables[0];
                foreach (DataRow row in dataTable.Rows)
                {
                    report.Add(new
                    {
                        SportType = row["SportType"],
                        SportName = row["SportName"],
                        WinLost = row["WinLost"],
                        Position = row["Position"],
                        Commission = row["Commission"],
                        RealLineTotal = row["RealLineTotal"],
                        VirtualLineTotal = row["VirtualLineTotal"],
                        RealTurnover = row["RealTurnover"],
                        VirtualTurnover = row["VirtualTurnover"],
                        ibc = row["ibc"],
                        Site = row["Site"]
                    });
                }
            }

            return report;
        }
    }
}
