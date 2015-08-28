// --------------------------------------------------------------------------
// <copyright file="IWinLossSummary3rdService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Kan.Nguyen" date="2013.06.25">Created</change>
// </history>

namespace Fanex.AccountingReports.Main.Services.Interface
{
    using Fanex.AccountingReports.Main.Criteria.WinLossSummary3rd;
    using Fanex.AccountingReports.Main.Models.WinLossSummary3rd;
    using Fanex.NAP.Framework.Data;

    public interface IWinLossSummary3rdService
    {
        WinLoss3rdInfo GetReport(CriteriaBase criteria);

        void ExportReport(GetReportCriteria criteria);

        void RecalculateReport(CriteriaBase criteria);

        MatchGame3rdInfo GetDetails(GetMatchGameDetailsCriteria criteria);

        void ExportDetails(GetMatchGameDetailsCriteria criteria);
    }
}
