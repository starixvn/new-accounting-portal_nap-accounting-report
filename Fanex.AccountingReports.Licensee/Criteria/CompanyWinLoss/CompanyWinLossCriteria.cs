namespace Fanex.AccountingReports.Licensee.Criteria.CompanyWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CompanyWinLossCriteria : CriteriaBase
    {
        public int LicenseeId { get; set; }

        public int CurrencyId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate
        {
            get
            {
                var endOfMonth = new DateTime(
                    this.FromDate.Year,
                    this.FromDate.Month,
                    DateTime.DaysInMonth(this.FromDate.Year, this.FromDate.Month));
                if (endOfMonth > DateTime.Now)
                {
                    endOfMonth = DateTime.Now;
                }

                return endOfMonth;
            }
        }

        public string ProductPrefix { get; set; }

        public DateTime MoveBetsDay
        {
            get;
            set;
        }

        [SpParam(Ignored = true)]
        public bool IsPagingCallBack { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        public override string GetSettingKey()
        {
            return "Acc_Rpt_CompanyWinloss";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
