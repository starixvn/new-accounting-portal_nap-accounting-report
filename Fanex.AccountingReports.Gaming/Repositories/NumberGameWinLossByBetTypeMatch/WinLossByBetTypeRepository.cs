namespace Fanex.AccountingReports.Gaming.Repositories.Implement.NumberGameWinLossByBetTypeMatch
{
    using System.Collections.Generic;
    using System.Data;
    using AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch;
    using Fanex.Common;
    using Fanex.Data;

    public class WinLossByBetTypeRepository
    {
        public List<WinLossInfo> GetMainReport(GetWinLossByBetTypeMainReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = ObjectDbFactory.CreateInstance(settingKey))
            {
                if (criteria.LiveStatus == -1)
                {
                    criteria.LiveStatus = null;
                }

                result = objectDb.ExecuteDataSet(criteria);
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
                    SportType = Converter.ToInt(row["SportType"]),
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

        public List<WinLossDetailInfo> GetDetailReport(GetWinLossByBetTypeDetailReportCriteria criteria)
        {
            DataSet result = null;
            var settingKey = criteria.GetSettingKey();
            using (var objectDb = ObjectDbFactory.CreateInstance(settingKey))
            {
                if (criteria.LiveStatus == -1)
                {
                    criteria.LiveStatus = null;
                }

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
                    BetType = Converter.ToInt(row["bType"]),
                    LeagueId = Converter.ToInt(row["leagueid"])
                };

                report.Add(info);
            }

            return report;
        }
    }
}
