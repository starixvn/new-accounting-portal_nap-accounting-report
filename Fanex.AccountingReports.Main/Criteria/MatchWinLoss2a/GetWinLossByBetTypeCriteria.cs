// -----------------------------------------------------------------------
// <copyright file="GetWinLossByBetTypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.02.19">Created</change>
// </history>
namespace Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get win loss by bet type criteria.
    /// </summary>
    public class GetWinLossByBetTypeCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        [SpParam(Ignored = true)]
        public bool IsGetMainReport { get; set; }

        [SpParam(Ignored = true)]
        public bool IsGetDetailReport { get; set; }

        public override string GetSettingKey()
        {
            return "Main_MatchWinloss2a_GetWinLossByBettype" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}