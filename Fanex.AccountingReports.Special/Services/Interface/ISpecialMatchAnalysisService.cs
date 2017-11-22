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
