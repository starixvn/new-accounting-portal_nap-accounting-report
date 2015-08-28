// --------------------------------------------------------------------------
// <copyright file="IDailyWinLoss2hRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2h;
    using Fanex.AccountingReports.Main.Models.DailyWinLoss2h;

    public interface IDailyWinLoss2hRepository
    {
        List<dynamic> GetMain2hReport(GetMain2hReportCriteria criteria, ref Hashtable outputParam);

        List<dynamic> GetCasinoReport(GetCasinoReportCriteria criteria, ref Hashtable outputParam);

        List<dynamic> GetSystemParlayReport(GetMixParlayReportCriteria criteria);

        List<dynamic> GetSummaryNumberGameReport(GetSummaryNumberGameReportCriteria criteria);

        List<dynamic> GetSummaryOtherSportReport(GetSummaryOtherSportReportCriteria criteria);

        List<dynamic> GetOtherSportReportBySport(GetOtherSportReportBySportCriteria criteria);

        List<dynamic> GetSoccerReport(GetSoccerReportCriteria criteria);

        List<dynamic> GetBingoReport(GetBingoReportCriteria criteria);

        List<dynamic> GetVirtualSportReport(GetVirtualSportReportCriteria criteria);

        List<dynamic> GetVirtualSportReportBySport(GetVirtualSportReportBySportCriteria criteria);

        List<dynamic> GetReportByBetType(GetReportByBetTypeCriteria criteria);

        List<dynamic> GetParlayReport(GetParlayReportCriteria criteria);

        ProductWinLoss GetMainLiveCasinoReport(GetLiveCasinoReportCriteria criteria);
    }
}