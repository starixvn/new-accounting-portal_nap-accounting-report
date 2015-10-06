// --------------------------------------------------------------------------
// <copyright file="GetWinLossBySite_BetTypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.AccountingReports.Main.Constants;
    using Fanex.NAP.Framework.Data;
    using CoreConstants = Fanex.AccountingReports.Core.Constants;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetWinLossBySite_BetTypeCriteria : CriteriaBase
    {
        public DateTime WinLostDate { get; set; }

        public int SportType { get; set; }

        public int BetType { get; set; }

        public bool LiveIndicator { get; set; }

        public bool IsHedge { get; set; }

        public int Site { get; set; }

        [SpParam(Ignored = true)]
        public string PreviousBreadScrum { get; set; }

        [SpParam(Ignored = true)]
        public int SportGroupId { get; set; }

        public int ProductId { get; set; }

        /// <summary>
        /// Gets the setting key.
        /// </summary>
        /// <returns>The setting key.</returns>
        public override string GetSettingKey()
        {
            var winLossDateSuffix = ReportService.GetStoreMappingSuffix(this.WinLostDate);
            if (winLossDateSuffix == ReportService.WarehouseSPSuffix)
            {
                return "Main_DailyWinLoss2h_WinLossBySiteBetType_WH";
            }

            var storeProcedureAliasName = "Main_DailyWinLoss2h_WinLossBySiteBetType";
            var sportNameSuffix = string.Empty;
            switch (this.SportGroupId)
            {
                case CoreConstants.SportContants.HorseRacing:
                    sportNameSuffix = "_Racing";
                    break;
                case CoreConstants.SportContants.LiveCasino:
                case SportContants._2hGoldDeluxeSport:
                case SportContants._2hLiveCasinoSport:
                case SportContants._2hAGCasinoSport:
                case SportContants._2hMicroGamingSport:
                    sportNameSuffix = "_LiveCasino";
                    break;
                case CoreConstants.SportContants.Keno:
                    sportNameSuffix = "_Keno";
                    break;
                case CoreConstants.SportContants.Bingo:
                    sportNameSuffix = "_Bingo";
                    break;
                case SportContants._2hReportCasino:
                    sportNameSuffix = "_Casino";
                    break;
                case CoreConstants.SportContants.VirtualSoccer:
                    sportNameSuffix = "_VirtualSports";
                    break;
                case CoreConstants.SportContants.MetricGamingSuperLive:
                    sportNameSuffix = "_MetricGaming";
                    break;
                case CoreConstants.SportContants.Bettrade:
                    sportNameSuffix = "_Bettrade";
                    break;
                case CoreConstants.SportContants.ColossusBet:
                    sportNameSuffix = "_ColossusBet";
                    break;
            }

            return string.Format("{0}{1}{2}", storeProcedureAliasName, sportNameSuffix, winLossDateSuffix);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
