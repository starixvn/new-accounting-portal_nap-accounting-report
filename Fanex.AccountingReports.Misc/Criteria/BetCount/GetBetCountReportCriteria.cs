// --------------------------------------------------------------------------
// <copyright file="GetBetCountReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Misc.Criteria.BetCount
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetBetCountReportCriteria : CriteriaBase
    {
        public GetBetCountReportCriteria()
        {
            this.Datestart = DateTime.Now.AddDays(-1);
            this.Dateend = this.Datestart;
        }

        public DateTime Datestart { get; set; }

        public DateTime Dateend { get; set; }

        public string Product { get; set; }

        [SpParam(Ignored = true)]
        public string SortBy { get; set; }

        [SpParam(Ignored = true)]
        public string SortDirection { get; set; }

        [SpParam(Ignored = true)]
        public bool IsHistoryReport { get; set; }

        public override string GetSettingKey()
        {
            return "Misc_BetCount_GetBetCountReport";
        }

        public override bool IsValid()
        {
            return this.Datestart <= this.Dateend;
        }
    }
}
