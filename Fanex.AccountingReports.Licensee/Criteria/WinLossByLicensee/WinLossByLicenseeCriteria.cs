// --------------------------------------------------------------------------
// <copyright file="WinLossByLicenseeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Bao" date="2012.11.26">Created</change>
// </history>

namespace Fanex.AccountingReports.Licensee.Criteria.WinLossByLicensee
{
    using System;
    using Fanex.AccountingReports.Core.Configurations;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class WinLossByLicenseeCriteria : CriteriaBase
    {
        [SpParam(Ignored = true)]
        public static int ConvertCurrencyId { get; set; }

        public int LicenseeID { get; set; }

        [SpParam(Ignored = true)]
        public int LevelReport { get; set; }

        [SpParam(Ignored = true)]
        public string LicenseeName { get; set; }

        public int Currency { get; set; }

        [SpParam(Ignored = true)]
        public string CurrencyName { get; set; }

        [SpParam(Ignored = true)]
        public string UserName { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool ShowSB { get; set; }

        public bool ShowRB { get; set; }

        public bool ShowNG { get; set; }

        public bool ShowBA { get; set; }

        public bool ShowLC { get; set; }

        public bool ShowVS { get; set; }

        public bool ShowCS { get; set; }

        public bool ShowKN { get; set; }

        public bool ShowAGC { get; set; }

        public bool ShowGD { get; set; }

        public bool ShowMGC { get; set; }

        public int Drawfilter { get; set; }

        [SpParam(Ignored = true)]
        public int ConvertToCurrency { get; set; }

        [SpParam(Ignored = true)]
        public string ConvertToCurrencyName { get; set; }

        public DateTime MoveBetsDay
        {
            get
            {
                return ReportService.BalanceUp.MoveBetsDay;
            }
        }

        /// <summary>
        /// Gets a value indicating whether [is old].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [is old]; otherwise, <c>false</c>.
        /// </value>
        public bool IsOld
        {
            get
            {
                return this.FromDate < AppSettings.WinLossByLicensee_OldTableBaseTime;
            }
        }

        public bool ExcludeTest
        {
            get
            {
                return true;
            }
        }

        #region PagingCriteria

        [SpParam(Ignored = true)]
        public bool IsGetAll { get; set; }

        [SpParam(Ignored = true)]
        public int ItemsPerPage { get; set; }

        [SpParam(Ignored = true)]
        public int TotalItem { get; set; }

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

        #endregion PagingCriteria
    }
}