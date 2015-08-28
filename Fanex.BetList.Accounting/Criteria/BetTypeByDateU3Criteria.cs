// -----------------------------------------------------------------------
// <copyright file="BetTypeByDateU3Criteria.cs" company="Microsoft">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.BetList.Accounting.Criteria
{
    using System;
    using Fanex.AccountingReports.Core.Web;
    using Fanex.BetList.Accounting.Services;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class BetTypeByDateU3Criteria : CriteriaBase
    {
        public BetTypeByDateU3Criteria()
        {
            MoveBetsDay = ReportService.BalanceUp.MoveBetsDay;
            BalanceUpDay = ReportService.BalanceUp.BalanceUpDay;
        }

        [SpParam(SqlParamName = "CustId")]
        public int CustId { get; set; }

        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime MoveBetsDay { get; set; }

        public DateTime BalanceUpDay { get; set; }

        public override string GetSettingKey()
        {
            return "U3_BetList_SelBettype";
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}