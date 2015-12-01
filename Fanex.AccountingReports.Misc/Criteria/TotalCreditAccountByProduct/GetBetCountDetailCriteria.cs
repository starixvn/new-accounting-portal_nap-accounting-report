// -----------------------------------------------------------------------
// <copyright file="GetBetCountDetailCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.06.10">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.TotalCreditAccountByProduct
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get bet count detail criteria.
    /// </summary>
    public class GetBetCountDetailCriteria : CriteriaBase
    {
        public DateTime MoveBetsDay { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public int CustID { get; set; }

        public int SportType { get; set; }

        [SpParam(Ignored = true)]
        public string SuperUserName { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        public override bool IsValid()
        {
            return this.MoveBetsDay <= this.BalanceUpDay;
        }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_BetCount_ByProduct_Super";
        }
    }
}