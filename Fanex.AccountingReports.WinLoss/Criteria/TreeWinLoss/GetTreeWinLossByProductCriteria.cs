// -----------------------------------------------------------------------
// <copyright file="GetTreeWinLossByProductCriteria.cs" company="Nexcel Solutions Vietnam">
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
    public class GetTreeWinLossByProductCriteria : CriteriaBase
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

        public bool ShowMGC { get; set; }

        public bool ShowVG { get; set; }

        public bool ShowAGCS { get; set; }

        public bool ShowCB { get; set; }

        public double Turnover_8RM { get; set; }

        public double Total_8RM { get; set; }

        public double VirtualTotal_8RM { get; set; }

        public double Turnover_BARM { get; set; }

        public double Total_BARM { get; set; }

        public double VirtualTotal_BARM { get; set; }

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
            if (FromDate <= movebetsDay && ToDate <= movebetsDay)
            {
                return "WinLoss_TreeWinLoss_GetDataByProduct_WH";
            }
            else if (FromDate > movebetsDay && ToDate > movebetsDay)
            {
                return "WinLoss_TreeWinLoss_GetDataByProduct";
            }

            return string.Empty;
        }
    }
}
