// --------------------------------------------------------------------------
// <copyright file="GetGameTypeListCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic
{
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetGameTypeListCriteria : CriteriaBase, ICriteria
    {
        public string ProductId { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_CasinoGameStatistics_GameTypeSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
