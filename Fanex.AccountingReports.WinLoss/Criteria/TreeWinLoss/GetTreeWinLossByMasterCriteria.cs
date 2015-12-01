namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetTreeWinLossByMasterCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public bool ShowSB { get; set; }

        public bool ShowNG { get; set; }

        public bool ShowRB { get; set; }

        public bool ShowLiveCasino { get; set; }

        public bool ShowCasino { get; set; }

        public bool ShowP2P { get; set; }

        public bool ShowBI { get; set; }

        public bool ShowVS { get; set; }

        public bool ShowKeno { get; set; }

        public bool ShowGD { get; set; }

        public bool ShowMGSL { get; set; }

        public bool ShowMGCLC { get; set; }

        public bool ShowMGCRNG { get; set; }

        public bool ShowVG { get; set; }

        public bool ShowAGCS { get; set; }

        public int MRecommend { get; set; }

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
            return "WinLoss_TreeWinLoss_GetDataByMaster" + storeProcedureSuffix;
        }
    }
}
