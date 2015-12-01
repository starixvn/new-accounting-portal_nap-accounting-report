// --------------------------------------------------------------------------
// <copyright file="GetMainReportCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.AGCasinoWinloss
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetMainReportCriteria : CriteriaBase
    {
        public int LicenseeID { get; set; }

        [SpParam(Ignored = true)]
        public string LicenseeName { get; set; }

        public int Currency { get; set; }

        [SpParam(Ignored = true)]
        public string CurrencyName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ReportType { get; set; }

        [SpParam(Ignored = true)]
        public int ConvertFiguresTo { get; set; }

        public bool ExcludeTest { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        [SpParam(Ignored = true)]
        public int IsRefresh { get; set; }

        public override string GetSettingKey()
        {
            return "Licensee_AGCasinoWinloss_GetMainReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}