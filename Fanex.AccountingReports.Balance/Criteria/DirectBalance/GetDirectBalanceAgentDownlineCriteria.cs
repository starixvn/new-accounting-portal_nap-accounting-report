// --------------------------------------------------------------------------
// <copyright file="GetDirectBalanceAgentDownlineCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Balance.Criteria.DirectBalance
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetDirectBalanceAgentDownlineCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public string CustomerType { get; set; }

        public int AgentId { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Balance_MasterBalance_MemberDownline";
        }
    }
}
