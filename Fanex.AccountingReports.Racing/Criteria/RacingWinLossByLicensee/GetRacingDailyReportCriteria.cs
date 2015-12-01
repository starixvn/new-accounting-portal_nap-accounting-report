// -----------------------------------------------------------------------
// <copyright file="GetRacingDailyReportCriteria.cs" company="Nexcel Solutions VietName">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Racing.Criteria.RacingWinLossByLicensee
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// The criteria of Racing Win Loss By Licensee.
    /// </summary>
    public class GetRacingDailyReportCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public string Site { get; set; }

        [SpParam(Ignored = true)]
        public bool IsPagingReport { get; set; }

        [SpParam(Ignored = true)]
        public int NumberItemsPerPage { get; set; }

        [SpParam(Ignored = true)]
        public int StartIndex { get; set; }

        /// <summary>
        /// Get setting key.
        /// </summary>
        /// <returns>Return key.</returns>
        public override string GetSettingKey()
        {
            string suffix = string.Empty;
            if (WinLostDate > ReportService.BalanceUp.MoveBetsDay)
            {
                suffix = string.Empty;
            }
            else
            {
                suffix = "_14";
            }

            return "Racing_RacingWinLostLicensee_Detail" + suffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}