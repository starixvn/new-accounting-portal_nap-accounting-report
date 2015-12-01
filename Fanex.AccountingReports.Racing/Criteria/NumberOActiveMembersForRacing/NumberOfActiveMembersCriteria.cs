// -----------------------------------------------------------------------
// <copyright file="NumberOfActiveMembersCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2013.12.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Racing.Criteria.NumberOActiveMembersForRacing
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Racing - Number Of Active Member Criteria.
    /// </summary>
    public class NumberOfActiveMembersCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_RacingActiveMember";
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}