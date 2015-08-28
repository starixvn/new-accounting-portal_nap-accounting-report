// --------------------------------------------------------------------------
// <copyright file="GetGameTableCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Phillip.Truong" date="2013.08.12">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Criteria.LiveCasinoGameFinancialReport
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetGameTableCriteria : CriteriaBase
    {
        public string GameType { get; set; }

        public override string GetSettingKey()
        {
            return "Gaming_LiveCasinoGameFinancial_GetGameTable";
        }

        public override bool IsValid()
        {
            return !string.IsNullOrWhiteSpace(this.GameType);
        }
    }
}
