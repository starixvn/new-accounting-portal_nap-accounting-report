// --------------------------------------------------------------------------
// <copyright file="GetCreditSitesBetCountsCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.CreditSitesBetCounts
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetCreditSitesBetCountsCriteria : CriteriaBase
    {
        public DateTime Datestart { get; set; }

        public DateTime Dateend { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_CreditSiteBetCount_GetReport";
        }

        public override bool IsValid()
        {
            return this.Datestart <= this.Dateend;
        }
    }
}