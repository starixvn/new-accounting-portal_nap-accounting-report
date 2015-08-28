// --------------------------------------------------------------------------
// <copyright file="IMatchWinLoss2aService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Interface
{
    using Fanex.AccountingReports.Main.Criteria.MatchWinLoss2a;
    using Fanex.AccountingReports.Main.Models.MatchWinLoss2a;

    public interface IMatchWinLoss2aService
    {
        dynamic GetMainReport(GetMainReportCriteria criteria);

        SummaryReportByBetType GetMainReportByBetType(GetWinLossByBetTypeCriteria criteria);

        WinLossInfo GetMainReportBySportType(GetMainReportCriteria criteria);

        WinLossInfo GetDetailReportByBetType(GetDetailReportByBetTypeCriteria criteria);

        WinLossInfo GetDetailReportBeforeBalanceUpDayByBetType(GetDetailReportByBetTypeCriteria criteria);

        WinLossInfo GetBetTypeDetailReportBySports(GetDetailReportByBetTypeCriteria criteria);

        WinLossInfo GetBetTypeDetailReportBySportsBeforeBalanceUpDay(GetDetailReportByBetTypeCriteria criteria);

        WinLossInfo GetThirdPartyCasinoReport(GetThirdPartyCasinoReportCriteria criteria);

        void ExportSummaryReportByBetType(GetMainReportCriteria criteria);

        void ExportSummaryReportBySportType(GetMainReportCriteria criteria);

        void ExportDetailReportByBetType(GetDetailReportByBetTypeCriteria criteria);

        void ExportDetailReportBySportsType(GetDetailReportByBetTypeCriteria criteria);

        void ExportAllDetailReport(GetMainReportCriteria criteria);
    }
}