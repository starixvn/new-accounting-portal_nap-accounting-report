// --------------------------------------------------------------------------
// <copyright file="GetGameTypeListCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.LiveCasinoWinLossByAgency
{
    using Fanex.AccountingReports.Core.Constants;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetGameTypeListCriteria : CriteriaBase, ICriteria
    {
        [SpParam(SqlParamName = "ProductID")]
        public int ProductId { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_LiveCasinoGameStatistics_GetGameTypeList";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}