namespace Fanex.AccountingReports.WinLoss.Repositories.Implement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.WinLoss.Models.TreeWinLoss;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class TreeWinLossRepository
    {
        public List<TreeWinLossInfo> GetTreeWinLossByProduct(ICriteria criteria, ref Hashtable outputParam)
        {
            var treeWinLoss = new List<TreeWinLossInfo>();
            DataSet result;
            using (var db = ObjectDbFactory.CreateInstance(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria);
                outputParam = db.GetOutputParams();
            }

            if (result != null && result.Tables.Count > 0)
            {
                var winLossTable = result.Tables[0];
                foreach (DataRow row in winLossTable.Rows)
                {
                    treeWinLoss.Add(new TreeWinLossInfo
                    {
                        Id = Converter.ToInt(row["currency"]),
                        Turnover = Converter.ToDecimal(row["turnover"]),
                        WinLoss = Converter.ToDecimal(row["winloss"]),
                        Comm = Converter.ToDecimal(row["commission"]),
                        WinLossTotal = Converter.ToDecimal(row["total"]),
                        BaseRMTurnover = Converter.ToDecimal(row["turnoverRM"]),
                        BaseRMTotal = Converter.ToDecimal(row["totalRM"]),
                        BaseRMVirtualTotal = Converter.ToDecimal(row["virtual_total"]),
                        BaseRMVirtualTurnover = Converter.ToDecimal(row["virtual_turnover"]),
                        BaseRMTOVarianceMinus = Converter.ToDecimal(row["TOVariance-"]),
                        BaseRMTOVariancePlus = Converter.ToDecimal(row["TOVariance+"]),
                        BaseRMWLVarianceMinus = Converter.ToDecimal(row["WLVariance-"]),
                        BaseRMWLVariancePlus = Converter.ToDecimal(row["WLVariance+"]),
                        ExtraAmount = Converter.ToDecimal(row["ExtraAmount"]),
                        BaseRMWinlostVarianceTotal = Converter.ToDecimal(row["WLVarianceTotal"]),
                        BaseRMTOVarianceAbsoluteTotal = Converter.ToDecimal(row["TOVarianceTotal"])
                    });
                }
            }

            return treeWinLoss;
        }
    }
}
