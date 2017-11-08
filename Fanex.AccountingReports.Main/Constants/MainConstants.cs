// --------------------------------------------------------------------------
// <copyright file="MainConstants.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Common
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MainConstants
    {
        public const int DailyWinLoss2hKickOffId = 1001;
        public const int WinLossSummaryMixParlaySportId = 97;
        public const int WinLossSummarySystemParlaySportId = 98;
        public const int WinLossSummaryTotalSportId = 0;
        public const int WinLossSummaryTotalBetTypeId = 19999;

        // Only for Web code (don't have this Id in DB)
        public const int WinLossSummaryRacingSportId = -100;

        public const int WinLossSummaryVirtualSportId = -200;

        public const int WinLossSummaryBetradarVirtualSportId = -201;

        public const int WinLossSummaryMaxSportbookId = 99;

        public const int WinLossSummaryKenoSportId = 202;

        // Range ID of Virtual Sport
        public const int WinLossSummaryBeginVirtualSportId = 180;

        public const int WinLossSummaryEndVirtualSportId = 186;

        // Range ID of Betradar Virtual Sports
        public const int WinLossSummaryBeginBetradarVirtualSportId = 190;

        public const int WinLossSummaryEndBetradarVirtualSportId = 199;

        // Sport Ids that don't need to show detail link
        public const int WinLossSumamryCasinoSportId = 251;

        public const int WinLossSummaryLiveCasinoSportId = 162;
        public const int WinLossSummaryBingoSportId = 252;
        public const int WinLossSummaryP2PSportId = 253;

        public const int Bettrade = 903;

        public const int MinMetricGamingSuperLiveBetType = 801;
        public const int MaxMetricGamingSuperLiveBetType = 899;

        public static readonly string DailyWinLoss2hMain2hReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.Main2hReport";
        public static readonly string DailyWinLoss2hCasinoReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.CasinoReport";
        public static readonly string DailyWinLoss2hLiveCasinoReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.LiveCasinoReport";
        public static readonly string DailyWinLoss2hSoccerReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.SoccerReport";
        public static readonly string DailyWinLoss2hBingoReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.BingoReport";

        public static readonly string DailyWinLoss2hMixParlayReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.SystemParlayReport";
        public static readonly string DailyWinLoss2hSummaryRacingReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.SummaryRacingReport";
        public static readonly string DailyWinLoss2hRacingReportBySportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.RacingReportBySport";
        public static readonly string DailyWinLoss2hRacingReportByBeTypeSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.RacingReportByBetType";

        public static readonly string DailyWinLoss2hSummaryNumberGameReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.SummaryNumberGameReport";
        public static readonly string DailyWinLoss2hReportByBetTypeSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.ReportByBetType";
        public static readonly string DailyWinLoss2hSummaryOtherSportReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.SummaryOtherSportReport";
        public static readonly string DailyWinLoss2hOtherSportReportBySportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.SummaryOtherSportReportBySport";

        public static readonly string DailyWinLoss2hVirtualSportReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.VirtualSportReport";
        public static readonly string DailyWinLoss2hVirtualSportReportBySportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.VirtualSportReportBySport";

        public static readonly string DailyWinLoss2hVirtualSportsVirtualHorseRacingReportSessionKey =
            "Fanex.AccountingReports.Main.DailyWinLoss2h.VirtualSportsVirtualHorseRacingReport";

        public static readonly string DailyWinLoss2hWinLossBySiteBetTypeReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2h.WinLossBySite.BetType";

        public static readonly string DailyWinLoss2hPopularLeagueNamePrefix = "*";

        public static readonly string DailyWinLoss2hSoccerMain2hReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2hSoccer.Main2hReport";
        public static readonly string DailyWinLoss2hOtherSportMain2hReportSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2hOtherSport.Main2hReport";

        public static readonly string TraderPerformanceReportSessionKey = "Fanex.AccountingReports.Main.TraderPerformance.Report";
        public static readonly string TraderPerformanceDetailReportSessionKey = "Fanex.AccountingReports.Main.TraderPerformance.DetailReport";

        public static readonly string WinLossSummary3rdReportSessionKey = "Fanex.AccountingReports.Main.WinLossSummary3rd.Report";
        public static readonly string WinLossSummary3rdReportDetailSessionKEy = "Fanex.AccountingReports.Main.WinLossSummary3rd.Details";

        public static readonly string DailyWinLoss2hSoccerMinorMainSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2hSoccerMinor.Main";
        public static readonly string DailyWinLoss2hSoccerMinorSporttypeSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2hSoccerMinor.Sporttype";
        public static readonly string DailyWinLoss2hSoccerMinorBettypeSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2hSoccerMinor.Bettype";
        public static readonly string DailyWinLoss2hSoccerMinorSiteSessionKey = "Fanex.AccountingReports.Main.DailyWinLoss2hSoccerMinor.Site";

        public static readonly int[] WinLossSummaryRacingSportIds = new int[] { 151, 152, 153 };
        public static readonly int[] WinLossSummaryVirtualRacingSportTypeIds = new int[] { 181, 182, 184, 183, 185 };
        public static readonly int[] WinLossSummaryVirtualSportIds = new int[] { 180, 186, 187, 188, 189, 190 };

        public static readonly short FontSize = 8;
        public static readonly string FontName = "Tahoma";
        public static readonly string TwoDecimalDigitNumber = "##,##0.00";
        private const string NegativeNumberFormat = "[>=0]#,##0{0}_);[RED][<0]-#,##0{1}";
        private const string NormalNumberFormat = "#,##0{0}_);-#,##0{1}";
        private const string PositiveNumberFormat = "[RED][>0]#,##0{0}_);[<0]-#,##0{1};#,##0{2}";

        public const int WinLossSummary3rdBullfightingSportId = 207;
    }
}