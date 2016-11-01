// --------------------------------------------------------------------------
// <copyright file="SpecialConstants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Constants
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class SpecialConstants
    {
        public const int PunterAnalysisSoccerGroupId = 0;
        public const int PunterAnalysisOtherSportGroupId = 1;
        public const int PunterAnalysisPArlayGroupId = 2;
        public const int PunterAnalysisSoocerSportId = 1;
        public const int PunterAnalysisOtherSportId = 999;
        public const int PunterAnalysisParlaySportId = 0;
        public const int PunterAnalysisNG2GroupID = 23;

        public static readonly string ParlayWonFullReportSessionKey = "Fanex.AccountingReports.Special.ParlayWon.ParlayWonReport";
        public static readonly string MatchAnalysisSessionKey = "Fanex.AccountingReports.Special.MatchAnalysis";

        public static readonly string PunterAnalysisSportsSessionKey = "Fanex.AccountingReports.Special.PunterAnalysis.Sports";
        public static readonly string PunterAnalysisTopPunterReportSessionKey = "Fanex.AccountingReports.Special.PunterAnalysis.TopPunterReport";
        public static readonly string PunterAnalysisTopPunterByUSerSessionKey = "Fanex.AccountingReports.Special.PunterAnalysis.TopPunterByUser";
        public static readonly string PunterAnalysisTopPunterBetCountByUserSessionKey = "Fanex.AccountingReports.Special.PunterAnalysis.TopPunterBetCount_ByUser";
        public static readonly string PunterAnalysisTopPunterIPByUserSessionKey = "Fanex.AccountingReports.Special.PunterAnalysis.TopPunterIP_ByUser";
        public static readonly string PunterAnalysisTopPunterBetCountByUserBetTypeSessionKey =
            "Fanex.AccountingReports.Special.PunterAnalysis.TopPunterBetCountByUserBetTypeSessionKey";

        public static readonly string PunterAnalysisTopPunterByUserSportTypeSessionKey = "Fanex.AccountingReports.Special.PunterAnalysis.TopPunter_ByUser_SportType";
        public static readonly string PunterAnalysisTopPunterByUserBetTypeSessionKey = "Fanex.AccountingReports.Special.PunterAnalysis.TopPunter_ByUser_BetType";
        public static readonly string PunterAnalysisTopPunterByLeagueSessionKey = "Fanex.AccountingReports.Special.PunterAnalysis.TopPunterByLeague";
        public static readonly string PunterAnalysisTopPunterByLeagueUserSessionKey = "Fanex.AccountingReports.Special.PunterAnalysis.TopPunterByLeague_User";

        public static readonly string PunterAnalysisMixParlaySport = "0_9";
        public static readonly string PunterAnalysisSystemParlaySport = "0_29";

        public static readonly string IntNumberFormat = "#,##0";

        public static readonly short FontSize = 8;

        public static readonly string FontName = "Tahoma";

        public static readonly List<string> ProductsHaveSport = new List<string>
        {
            "sportsbook",
            "ng2"
        };
    }
}
