// --------------------------------------------------------------------------
// <copyright file="GetGameTableListCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetGameTableListCriteria : CriteriaBase, ICriteria
    {
        [SpParam(SqlParamName = "GameTypeID")]
        public string GameTypeId { get; set; }

        public bool ExcludeTest { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_CasinoGameStatistics_GameTableSel";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}