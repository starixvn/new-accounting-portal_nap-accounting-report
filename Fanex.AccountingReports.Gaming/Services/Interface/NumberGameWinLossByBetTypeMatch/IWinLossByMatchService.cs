// --------------------------------------------------------------------------
// <copyright file="IWinLossByMatchService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Gaming.Services.Interface.NumberGameWinLossByBetTypeMatch
{
    using Fanex.AccountingReports.Gaming.Criteria.NumberGameWinLossByBetTypeMatch;
    using Fanex.AccountingReports.Gaming.Models.NumberGameWinLossByBetTypeMatch;

    public interface IWinLossByMatchService
    {
        WinLossInfo GetMainReport(GetWinLossByMatchMainReportCriteria criteria);

        WinLossInfo GetDetailReport(GetWinLossByMatchDetailReportCriteria criteria);

        void ExportMainReport(GetWinLossByMatchMainReportCriteria criteria);

        void ExportDetailReport(GetWinLossByMatchDetailReportCriteria criteria);
    }
}
