// --------------------------------------------------------------------------
// <copyright file="ISoccerReportService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------

namespace Fanex.AccountingReports.Main.Services.Interface.DailyWinLoss2hSoccerMinorBettype
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Main.Criteria.DailyWinLoss2hSoccerMinorBettype;
    using Fanex.Excel.Models;

    public interface ISoccerReportService
    {
        List<GridOptions> GetReport(GetSoccerReportCriteria criteria, bool isCached = false);
    }
}
