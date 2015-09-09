// -----------------------------------------------------------------------
// <copyright file="GetProductCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Gaming.Criteria.NumberOfActiveMember
{
    using Fanex.Data.Repository;

    /// <summary>
    /// Get product criteria.
    /// </summary>
    public class GetProductCriteria : CriteriaBase
    {
        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Gaming_NumberOfActiveMember_Products";
        }
    }
}