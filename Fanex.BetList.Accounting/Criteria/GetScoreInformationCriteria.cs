// --------------------------------------------------------------------------
// <copyright file="GetScoreInformationCriteria.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.BetList.Accounting.Criteria
{
    using System;
    using Fanex.BetList.Accounting.Services;
    using Fanex.NAP.Framework.Data;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetScoreInformationCriteria : CriteriaBase
    {
        [SpParam(SqlParamName = "MatchID")]
        public int MatchId { get; set; }

        public int Bettype { get; set; }

        public string TraderName { get; set; }

        public DateTime ReportDate { get; set; }

        public int SportType { get; set; }

        public string SportTypeName { get; set; }

        public string BetTypeName { get; set; }

        public int LiveIndicator { get; set; }

        public string Hedge { get; set; }

        public int LeagueId { get; set; }

        public string LeagueName { get; set; }

        public string MatchCode { get; set; }

        public int HomeTeam { get; set; }

        public string HomeTeamName { get; set; }

        public int AwayTeam { get; set; }

        public string AwayTeamName { get; set; }

        public string BetId { get; set; }

        public override string GetSettingKey()
        {
            return "BetList_ScoreInformation_SelScoreInformation" +
                (this.ReportDate < ReportService.BalanceUp.BalanceUpDay ?
                 ReportService.WarehouseSPSuffix :
                 string.Empty);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}