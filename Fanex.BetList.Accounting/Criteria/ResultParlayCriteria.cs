// -----------------------------------------------------------------------
// <copyright file="ResultParlayCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ResultParlayCriteria : CriteriaBase 
    {
        [SpParam(SqlParamName = "refno")]
        public long RefNo { get; set; }

        public override string GetSettingKey()
        {
            return "Age_BetList_SelMatchResultParlay";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}