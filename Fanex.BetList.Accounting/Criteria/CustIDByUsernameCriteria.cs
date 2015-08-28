// -----------------------------------------------------------------------
// <copyright file="CustIDByUsernameCriteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustIDByUsernameCriteria : CriteriaBase, ICriteria
    {
        [SpParam(SqlParamName = "custname")]
        public string CustomerName { get; set; }

        public override string GetSettingKey()
        {
            return "GetCustIdByUsername";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}