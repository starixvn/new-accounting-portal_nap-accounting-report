// -----------------------------------------------------------------------
// <copyright file="ITraderPerformanceSoccerRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.01.28">Created</change>
// </history>
namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Models.TraderPerformanceSoccer;

    public interface ITraderPerformanceSoccerRepository
    {
        TraderPerformanceSoccer GetTraderPerformanceSoccerReport(GetReportCriteria criteria);

        TraderPerformanceSoccerDetail GetTraderPerformanceSoccerDetailReport(GetDetailReportCriteria criteria);
    }
}