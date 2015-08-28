// --------------------------------------------------------------------------
// <copyright file="GetThirdPartyCasinoReportCriteria.cs" company="Nexcel Solutions Vietnam">
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
    /// Get third party casino report criteria.
    /// </summary>
    public class GetThirdPartyCasinoReportCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        public override string GetSettingKey()
        {
            return "Main_MatchWinLoss2a_GetThirdPartyCasinoReport" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
