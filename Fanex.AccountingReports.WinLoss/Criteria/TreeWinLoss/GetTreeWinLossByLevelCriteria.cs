// -----------------------------------------------------------------------
// <copyright file="GetTreeWinLossByLevelCriteria.cs" company="Nexcel Solutions Vietnam">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.AccountingReports.WinLoss.Criteria.TreeWinLoss
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetTreeWinLossByLevelCriteria : CriteriaBase
    {
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public int Currency { get; set; }

        [SpParam(SqlParamName = "srecommend")]
        public int SRecommend { get; set; }

        [SpParam(SqlParamName = "mrecommend")]
        public int MRecommend { get; set; }

        public int Recommend { get; set; }

        public int CustId { get; set; }

        [SpParam(Ignored = true)]
        public string Name { get; set; }

        [SpParam(Ignored = true)]
        public int RoleId { get; set; }

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

        public override bool IsValid()
        {
            return true;
        }

        /// <summary>
        /// Get store procedure.
        /// </summary>
        /// <returns>The store procedure name.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules",
        "SP2101:MethodMustNotContainMoreLinesThan",
        Justification = "Reviewed. Suppression is OK here.")]
        public override string GetSettingKey()
        {
            DateTime movebetsDay = ReportService.BalanceUp.MoveBetsDay;
            switch (this.RoleId)
            {
                case 2:
                    if (FromDate > movebetsDay && ToDate > movebetsDay)
                    {
                        return "WinLoss_TreeWinLoss_GetDataByAgent";
                    }
                    else if (FromDate <= movebetsDay && ToDate <= movebetsDay)
                    {
                        return "WinLoss_TreeWinLoss_GetDataByAgent_WH";
                    }

                    break;
                case 3:
                    if (FromDate > movebetsDay && ToDate > movebetsDay)
                    {
                        return "WinLoss_TreeWinLoss_GetDataByMaster";
                    }
                    else if (FromDate <= movebetsDay && ToDate <= movebetsDay)
                    {
                        return "WinLoss_TreeWinLoss_GetDataByMaster_WH";
                    }

                    break;
                case 4:
                    if (FromDate > movebetsDay && ToDate > movebetsDay)
                    {
                        return "WinLoss_TreeWinLoss_GetDataBySuper";
                    }
                    else if (FromDate <= movebetsDay && ToDate <= movebetsDay)
                    {
                        return "WinLoss_TreeWinLoss_GetDataBySuper_WH";
                    }

                    break;
                case 5:
                    if (FromDate > movebetsDay && ToDate > movebetsDay)
                    {
                        return "WinLoss_TreeWinLoss_GetDataByCom";
                    }
                    else if (FromDate <= movebetsDay && ToDate <= movebetsDay)
                    {
                        return "WinLoss_TreeWinLoss_GetDataByCom_WH";
                    }

                    break;
            }

            return string.Empty;
        }
    }
}
