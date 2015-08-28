// --------------------------------------------------------------------------
// <copyright file="GetMain2hReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccer
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMain2hReportCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public double LineTotal { get; set; }

        public override string GetSettingKey()
        {
            return "Main_DailyWinLoss2hSoccer_Main2hReport" + ReportService.GetStoreMappingSuffix(this.Winlostdate);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
