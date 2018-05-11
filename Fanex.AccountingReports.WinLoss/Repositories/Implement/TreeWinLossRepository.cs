namespace Fanex.AccountingReports.WinLoss.Repositories.Implement
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.AccountingReports.WinLoss.Models.TreeWinLoss;
    using Fanex.AccountingReports.WinLoss.Repositories.Interface;
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class TreeWinLossRepository : ITreeWinLossRepository
    {
        /// <summary>
        /// Get the tree win loss.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <param name="outputParam">The output Parameter.</param>
        /// <returns>The tree win loss.</returns>
        public List<TreeWinLossInfo> GetTreeWinLossByProduct(ICriteria criteria, ref Hashtable outputParam)
        {
            var treeWinLoss = new List<TreeWinLossInfo>();
            DataSet result;
            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                result = db.ExecuteDataSet(criteria.GetSpParams());
                outputParam = db.GetOuputParams();
            }

            /*ObjectDb db = new ObjectDb(criteria.GetSettingKey());
            var parameters = criteria.GetSpParams();
            result = db.ExecuteReader(parameters,);
            return result;*/
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