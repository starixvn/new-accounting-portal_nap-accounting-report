namespace Fanex.AccountingReports.WinLoss.Criteria.GetAllCustomerTurnOver
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetAllCustomerTurnOverCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string Site { get; set; }

        public string From { get; set; }

        [SpParam(Ignored = true)]
        public bool IsPagingCallBack { get; set; }

        [SpParam(Ignored = true)]
        public int StartIndex { get; set; }

        [SpParam(Ignored = true)]
        public int UsersPerPage { get; set; }

        public override string GetSettingKey()
        {
            return "WinLoss_AllCustomerTurnOver_GetReport";
        }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }
    }
}