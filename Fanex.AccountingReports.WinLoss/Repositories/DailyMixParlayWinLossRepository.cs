namespace Fanex.AccountingReports.WinLoss.Repositories.Implement
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.WinLoss.Criteria.DailyMixParlayWinLoss;
    using Fanex.Data;

    public class DailyMixParlayWinLossRepository
    {
        public List<dynamic> GetCombinationWinLossReport(GetCombinationWinLossReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = ObjectDbFactory.CreateInstance(settingKey))
            {
                result = objectDb.ExecuteDataSet(criteria);
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
                        MemberWinLoss = row["MemberWinLoss"],
                        MemberTurnover = row["MemberTurnover"],
                        BetCount = row["BetCount"],
                        SMAWinLoss = row["SMAWinLoss"],
                        Commission = row["Commission"],
                        RealIncome = row["RealIncome"],
                        RealTurnover = row["RealTurnover"],
                        Combinations = row["Combinations"],
                        ExtraPT = row["ExtraPT"],
                        MemberBonus = row["MemberBonus"],
                        HouseBonus = row["HouseBonus"]
                    });
                }
            }

            return report;
        }
    }
}
