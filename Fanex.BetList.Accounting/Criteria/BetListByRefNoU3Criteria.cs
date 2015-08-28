// --------------------------------------------------------------------------
// <copyright file="BetListByRefNoU3Criteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Criteria
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetListByRefNoU3Criteria : CriteriaBase
    {
        [SpParam(SqlParamName = "TransID")]
        public long TransId { get; set; }

        public override string GetSettingKey()
        {
            return "U3_BetsSelBetList_ByTransID_Advance";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}