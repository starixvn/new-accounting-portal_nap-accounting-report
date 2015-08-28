// -----------------------------------------------------------------------
// <copyright file="BetTypeByDateCriteria.cs" company="Microsoft">
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
    public class BetTypeByDateCriteria : CriteriaBase
    {
        private string _settingKey = "Acc_BetList_SelBettype";

        public BetTypeByDateCriteria()
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
            return _settingKey;
        }

        public override bool IsValid()
        {
            return true;
        }

        public void SetSettingKeyForU3()
        {
            this._settingKey = "U3_BetList_SelBettype";
        }
    }
}