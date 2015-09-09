// --------------------------------------------------------------------------
// <copyright file="GetRecalculateWinLossCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.07.03">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetRecalculateWinLossCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public override string GetSettingKey()
        {
            return "Main_WinLossSummary3rd_Exec_CalculateWinLoss";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
