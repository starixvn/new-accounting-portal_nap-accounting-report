// -----------------------------------------------------------------------
// <copyright file="GetTreeWinLossByLevelCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
