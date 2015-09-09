// --------------------------------------------------------------------------
// <copyright file="GetGameTypesCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2013.08.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetGameTypesCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Gaming_LiveCasinoGameFinancial_GetGameType";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}