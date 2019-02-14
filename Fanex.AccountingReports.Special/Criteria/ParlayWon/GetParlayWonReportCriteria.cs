namespace Fanex.AccountingReports.Special.Criteria.ParlayWon
{
    using System;
    using Data;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetParlayWonReportCriteria : CriteriaBase
    {
        public DateTime WinlostDate { get; set; }

        public int Bettype { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public bool IsSoccer { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        public DateTime MoveBetsDay
        {
            get
            {
                return ReportService.BalanceUp.MoveBetsDay;
            }
        }

        public DateTime BalanceUpDay
        {
            get
            {
                return ReportService.BalanceUp.BalanceUpDay;
            }
        }

        [SpParam(Ignored = true)]
        public string SortBy { get; set; }

        [SpParam(Ignored = true)]
        public string SortDirection { get; set; }

        //// PagingCriteria

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

        [SpParam(Ignored = true)]
        public bool IsPagingCallBack { get; set; }

        //// PagingCriteria

        public override string GetSettingKey()
        {
            return "Special_ParlayWon_GetReport";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
