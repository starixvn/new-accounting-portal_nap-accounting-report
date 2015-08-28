// --------------------------------------------------------------------------
// <copyright file="GetMainReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get main report criteria.
    /// </summary>
    public class GetMainReportCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public double WinLost_8RM { get; set; }

        public double WinLost_BARM { get; set; }

        [SpParam(Ignored = true)]
        public bool IsGetMainReport { get; set; }

        [SpParam(Ignored = true)]
        public bool IsGetDetailReport { get; set; }

        public override string GetSettingKey()
        {
            return "Main_MatchWinLoss2a_GetMainReport" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
