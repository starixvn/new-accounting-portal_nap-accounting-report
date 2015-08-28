// --------------------------------------------------------------------------
// <copyright file="ISpecialMatchAnalysisService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Louis.Nguyen" date="2012.12.07">Created</change>
// </history>

namespace Fanex.AccountingReports.Special.Services.Interface
{
    using System.Collections.Generic;
    using Fanex.AccountingReports.Special.Criteria.SpecialMatchAnalysis;
    using Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis;

    public interface ISpecialMatchAnalysisService
    {
        List<SportLeague> GetSportLeague(GetSportLeagueCriteria criteria);

        List<LeagueMatch> GetLeagueMatch(GetLeageMatchCriteria criteria);

        List<Bettype> GetBetType(GetBetTypeCriteria criteria);

        MatchAnalysisView GetMatchAnalysis(GetMatchAnalysisCriteria criteria);

        void ExportReport(GetMatchAnalysisCriteria criteria);
    }
}
