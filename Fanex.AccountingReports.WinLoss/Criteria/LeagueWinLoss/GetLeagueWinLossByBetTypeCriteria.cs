// --------------------------------------------------------------------------
// <copyright file="GetLeagueWinLossByBetTypeCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.WinLoss.Criteria.LeagueWinLoss
{
    using System;
    using Fanex.AccountingReports.Core.Services.Impl;
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetLeagueWinLossByBetTypeCriteria : CriteriaBase
    {
        public DateTime Winlostdate { get; set; }

        public int Leagueid { get; set; }

        [SpParam(Ignored = true)]
        public string LeagueName { get; set; }

        public int Sporttype { get; set; }

        [SpParam(Ignored = true)]
        public string SportName { get; set; }

        public int Bettype { get; set; }

        [SpParam(Ignored = true)]
        public string BetTypeName { get; set; }

        public bool Liveindicator { get; set; }

        public bool IsHedge { get; set; }

        public double WinLost_8RM
        {
            get
            {
                return 0;
            }
        }

        public double WinLost_BARM
        {
            get
            {
                return 0;
            }
        }

        public override string GetSettingKey()
        {
            var storeProcedureSuffix = ReportService.GetStoreMappingSuffix(Winlostdate);
            if (storeProcedureSuffix == ReportService.WarehouseSPSuffix)
            {
                storeProcedureSuffix = ReportService.BetTrans14SPSuffix;
            }

            return "WinLoss_LeagueWinLoss_GetLeagueWinLossByBetType" + storeProcedureSuffix;
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
