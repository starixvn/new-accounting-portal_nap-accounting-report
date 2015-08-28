// --------------------------------------------------------------------------
// <copyright file="IParlayWonService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Interface
{
    using System.Diagnostics.CodeAnalysis;
    using Fanex.AccountingReports.Special.Criteria.ParlayWon;
    using Fanex.AccountingReports.Special.Models.ParlayWon;

    public interface IParlayWonService
    {
        ParlayWonTransaction GetReport(GetParlayWonReportCriteria criteria);

        ParlayWonTransaction SortReport(GetParlayWonReportCriteria criteria);

        void ExportReport(GetParlayWonReportCriteria criteria);
    }
}
