// --------------------------------------------------------------------------
// <copyright file="IDailyWinLoss2hService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Interface
{
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;

    public interface IDailyWinLoss2hService
    {
        WinLossInfo GetMain2hReport(GetMain2hReportCriteria criteria);

        LiveAndNoneLiveReport GetCasinoReport(GetCasinoReportCriteria criteria);

        WinLossInfo GetLiveCasinoReport(GetLiveCasinoReportCriteria criteria);

        WinLossInfo GetKenoReport(GetKenoReportCriteria criteria);

        WinLossInfo GetMixParlayReport(GetMixParlayReportCriteria criteria);

        WinLossInfo GetSummaryRacingReport(GetSummaryRacingReportCriteria criteria);

        WinLossInfo GetRacingReportBySport(GetRacingReportBySportCriteria criteria);

        WinLossInfo GetRacingReportByBetType(GetRacingReportByBetTypeCriteria criteria);

        LiveAndNoneLiveReport GetSummaryNumberGameReport(GetSummaryNumberGameReportCriteria criteria);

        LiveAndNoneLiveReport GetSummaryOtherSportReport(GetSummaryOtherSportReportCriteria criteria);

        LiveAndNoneLiveReport GetOtherSportReportBySport(GetOtherSportReportBySportCriteria criteria);

        LiveAndNoneLiveReport GetSoccerReport(GetSoccerReportCriteria criteria);

        LiveAndNoneLiveReport GetBingoReport(GetBingoReportCriteria criteria);

        LiveAndNoneLiveReport GetVirtualSportReport(GetVirtualSportReportCriteria criteria);

        LiveAndNoneLiveReport GetVirtualSportReportBySport(GetVirtualSportReportBySportCriteria criteria);

        WinLossInfo GetVirtualSports_VirtualHorseRacingReport(GetVirtualSports_VirtualHorseRacingReportCriteria criteria);

        WinLossInfo GetReportByBetType(GetReportByBetTypeCriteria criteria);

        WinLossInfo GetWinLossBySite_BetType(GetWinLossBySite_BetTypeCriteria criteria);

        WinLossInfo GetParlayReport(GetParlayReportCriteria criteria);

        void ExportMain2hReport(GetMain2hReportCriteria criteria);

        void ExportSummaryRacingReport(GetSummaryRacingReportCriteria criteria);

        void ExportRacingReportBySport(GetRacingReportBySportCriteria criteria);

        void ExportRacingReportByBetType(GetRacingReportByBetTypeCriteria criteria);

        void ExportCasinoReport(GetCasinoReportCriteria criteria);

        void ExportLiveCasinoReport(GetLiveCasinoReportCriteria criteria);

        void ExportKenoReport(GetKenoReportCriteria criteria);

        void ExportBingoReport(GetBingoReportCriteria criteria);

        void ExportNumberGameReport(GetSummaryNumberGameReportCriteria criteria);

        void ExportNumberGameReportByBetType(GetReportByBetTypeCriteria criteria);

        void ExportSummaryOtherSportReport(GetSummaryOtherSportReportCriteria criteria);

        void ExportOtherSportReportBySport(GetOtherSportReportBySportCriteria criteria);

        void ExportOtherSportReportByBetType(GetReportByBetTypeCriteria criteria);

        void ExportSoccerReport(GetSoccerReportCriteria criteria);

        void ExportSoccerReportByBetType(GetReportByBetTypeCriteria criteria);

        void ExportSystemParlayReport(GetMixParlayReportCriteria criteria);

        void ExportVirtualSportReport(GetVirtualSportReportCriteria criteria);

        void ExportVirtualSportReportBySport(GetVirtualSportReportBySportCriteria criteria);

        void ExportVirtualSportReportByBetType(GetReportByBetTypeCriteria criteria);

        void ExportVirtualSports_VirtualHorseRacingReport(GetVirtualSports_VirtualHorseRacingReportCriteria criteria);

        void ExportMixParlayReport(GetMixParlayReportCriteria criteria);

        void ExportWinLossBySite_BetType(GetWinLossBySite_BetTypeCriteria criteria);

        void ExportParlayReport(GetParlayReportCriteria criteria);

        WinLossInfo GetMainLiveCasinoReport(GetLiveCasinoReportCriteria criteria);

        void ExportMainLiveCasinoReport(GetLiveCasinoReportCriteria criteria);

        WinLossInfo GetLiveCasinoWinLossByProductReport(GetLiveCasinoWinLossByProductCriteria criteria);

        void ExportGetLiveCasinoWinLossByProductReport(GetLiveCasinoWinLossByProductCriteria criteria);
    }
}
