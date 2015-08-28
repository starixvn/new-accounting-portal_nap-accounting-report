// -----------------------------------------------------------------------
// <copyright file="ITraderPerformanceOtherSportsRepository.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
// <history>
//   <change who="Franklin.Nguyen" date="2014.01.28">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Repositories.Interface
{
    using Fanex.AccountingReports.Main.Criteria.TraderPerformance;
    using Fanex.AccountingReports.Main.Models.TraderPerformanceOtherSports;

    public interface ITraderPerformanceOtherSportsRepository
    {
        TraderPerformanceOtherSports GetTraderPerformanceOtherSportsReport(GetReportCriteria criteria);

        TraderPerformanceOtherSportsDetail GetTraderPerformanceOtherSportsDetailReport(GetDetailReportCriteria criteria);
    }
}