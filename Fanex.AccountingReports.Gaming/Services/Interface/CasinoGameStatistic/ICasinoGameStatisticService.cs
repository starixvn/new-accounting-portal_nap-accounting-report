// --------------------------------------------------------------------------
// <copyright file="ILiveCasinoGameStatisticService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Interface.CasinoGameStatistic
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Core.Criteria;
    using Fanex.AccountingReports.Core.Models;
    using Fanex.AccountingReports.Gaming.Criteria.CasinoGameStatistic;
    using Fanex.AccountingReports.Gaming.Models.CasinoGameStatistic;

    public interface ICasinoGameStatisticService
    {
        WinLossInfo GetReport(GetReportByDailyCriteria criteria);

        WinLossInfo SortReport(GetReportByDailyCriteria getReportCriteria);

        void ExportReport(GetReportByDailyCriteria criteria);

        WinLossInfo GetReportByGrandTotal(GetReportByGrandRMCriteria criteria);

        Hourly GetReportByHourly(GetReportByHourlyCriteria criteria);
    }
}