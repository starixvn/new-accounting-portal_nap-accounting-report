// --------------------------------------------------------------------------
// <copyright file="GetGameTypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2012.11.8">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.LiveCasinoWinLoss
{
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetGameTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "Licensee_LiveCasinoWinLoss_GetGameType";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
