// --------------------------------------------------------------------------
// <copyright file="IBingoStatisticService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Interface
{
    using Fanex.AccountingReports.Gaming.Criteria.BingoStatistic;
    using Fanex.AccountingReports.Gaming.Models.BingoStatistic;

    public interface IBingoStatisticService
    {
        WinLossInfo GetReport(GetReportCriteria criteria);

        void ExportReport(GetReportCriteria criteria);
    }
}
