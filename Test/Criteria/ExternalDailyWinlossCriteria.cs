// --------------------------------------------------------------------------
// <copyright file="ExternalDailyWinlossCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="" date="2013.9.24">Created</change>
// </history>

namespace Fanex.AccountingReports.Test.Criteria
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExternalDailyWinlossCriteria : CriteriaBase, ICriteria
    {
        public DateTime Winlostdate { get; set; }

        public int? Custid { get; set; }

        public string GroupName { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "External_DailyWinLoss_GetReport";
        }
    }
}