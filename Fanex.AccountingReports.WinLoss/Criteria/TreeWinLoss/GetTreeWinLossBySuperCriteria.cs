namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetTreeWinLossBySuperCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        [SpParam(Ignored = true)]
        public string ProductSelection { get; set; }

        public string Products { get; set; }

        public int SRecommend { get; set; }

        public override bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Get store procedure.
        /// </summary>
        /// <returns>The store procedure name.</returns>
        public override string GetSettingKey()
        {
            DateTime movebetsDay = ReportService.BalanceUp.MoveBetsDay;
            var storeProcedureSuffix = FromDate > movebetsDay && ToDate > movebetsDay ?
                string.Empty :
                ReportService.WarehouseSPSuffix;
            return "WinLoss_TreeWinLoss_GetDataBySuper" + storeProcedureSuffix;
        }
    }
}
