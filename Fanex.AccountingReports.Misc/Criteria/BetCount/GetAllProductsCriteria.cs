// --------------------------------------------------------------------------
// <copyright file="GetAllProductsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.BetCount
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetAllProductsCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Misc_BetCount_GetAllProductsForBetCountReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}