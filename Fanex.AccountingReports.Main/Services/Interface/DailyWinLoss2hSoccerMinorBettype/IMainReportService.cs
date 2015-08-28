// --------------------------------------------------------------------------
// <copyright file="IMainReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2hSoccerMinorBettype
{
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.Excel.Models;

    public interface IMainReportService
    {
        GridOptions GetReport(GetMainReportCriteria criteria, bool isCached = false);
    }
}
