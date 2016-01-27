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

        [SpParam(Ignored = true)]
        public string ProductSelection { get; set; }

        public string Products { get; set; }

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
