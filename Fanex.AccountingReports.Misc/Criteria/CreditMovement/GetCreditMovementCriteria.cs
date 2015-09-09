// -----------------------------------------------------------------------
// <copyright file="GetCreditMovementCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.Misc.Criteria.CreditMovement
{
    using System;
    using Fanex.Data.Repository;

    /// <summary>
    /// Get credit movement criteria.
    /// </summary>
    public class GetCreditMovementCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public string SiteList { get; set; }

        public long Amount { get; set; }

        [SpParam(Ignored = true)]
        public bool IsPagingCallBack { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        public override bool IsValid()
        {
            return this.FromDate <= this.ToDate;
        }

        public override string GetSettingKey()
        {
            return "Misc_CreditMovement_GetReport";
        }
    }
}