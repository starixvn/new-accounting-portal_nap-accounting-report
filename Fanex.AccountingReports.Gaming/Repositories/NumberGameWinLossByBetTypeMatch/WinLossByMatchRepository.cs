namespace Fanex.AccountingReports.Gaming.Repositories.Implement.NumberGameWinLossByBetTypeMatch
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch;
    using Fanex.Common;
    using Fanex.Data;

    public class WinLossByMatchRepository
    {
        public List<WinLossDetailInfo> GetMainReport(GetWinLossByMatchMainReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = ObjectDbFactory.CreateInstance(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria);
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
                    MatchId = Converter.ToInt(row["matchid"]),
                    SportType = Converter.ToInt(row["SportType"]),
                    BetType = Converter.ToInt(row["bettype"])
                };

                report.Add(info);
            }

            return report;
        }
    }
}
