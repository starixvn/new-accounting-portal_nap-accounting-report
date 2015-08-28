// -----------------------------------------------------------------------
// <copyright file="ITraderPerformanceSoccerService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.01.28">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Interface
{
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Models.TraderPerformanceSoccer;

    public interface ITraderPerformanceSoccerService
    {
        TraderPerformanceSoccer GetReport(GetReportCriteria criteria);

        TraderPerformanceSoccer SortReportData(GetReportCriteria criteria);

        void ExportReport(GetReportCriteria criteria);

        TraderPerformanceSoccerDetail GetDetailReport(GetDetailReportCriteria criteria);

        TraderPerformanceSoccerDetail SortDetailReportData(GetDetailReportCriteria criteria);

        void ExportDetailReport(GetDetailReportCriteria criteria);
    }
}