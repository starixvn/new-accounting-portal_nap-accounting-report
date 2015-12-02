// -----------------------------------------------------------------------
// <copyright file="GetProductList.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetProductListCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Acc_Rpt_CasinoGameStatistics_ProductSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}