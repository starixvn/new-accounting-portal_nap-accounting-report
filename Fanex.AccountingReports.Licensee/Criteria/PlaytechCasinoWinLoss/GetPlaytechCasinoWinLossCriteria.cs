// -----------------------------------------------------------------------
// <copyright file="GetPlaytechCasinoWinLossCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.04.14">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.PlaytechCasinoWinLoss
{
    using System;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// Get play tech casino win loss criteria.
    /// </summary>
    public class GetPlaytechCasinoWinLossCriteria : CriteriaBase
    {
        public int LicenseeID { get; set; }

        public int Currency { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int ReportType { get; set; }

        [SpParam(Ignored = true)]
        public int ConvertToCurrency { get; set; }

        [SpParam(SqlParamName = "excludeTest")]
        public bool ExcludeTestData { get; set; }

        [SpParam(Ignored = true)]
        public int ItemsPerPage { get; set; }

        [SpParam(Ignored = true)]
        public int CurrentPage { get; set; }

        /// <summary>
        /// Gets StartIndex: base from zero.
        /// </summary>
        /// <value>
        /// The start index.
        /// </value>
        [SpParam(Ignored = true)]
        public int StartIndex
        {
            get
            {
                if (this.CurrentPage <= 0)
                {
                    this.CurrentPage = 1;
                }

                return (this.CurrentPage - 1) * this.ItemsPerPage;
            }
        }

        /// <summary>
        /// Gets EndIndex: base from zero.
        /// </summary>
        /// <value>
        /// The end index.
        /// </value>
        [SpParam(Ignored = true)]
        public int EndIndex
        {
            get
            {
                return this.StartIndex + this.ItemsPerPage - 1;
            }
        }

        public override bool IsValid()
        {
            return true;
        }

        public override string GetSettingKey()
        {
            return "Licensee_PlaytechCasinoWinLoss_GetWinLossReport";
        }
    }
}