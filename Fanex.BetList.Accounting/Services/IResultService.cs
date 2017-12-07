namespace Fanex.BetList.Accounting.Services
{
    using System.Collections.Generic;
    using Fanex.BetList.Accounting.Models;
    using Fanex.Data.Repository;

    public interface IResultService
    {
        List<TennisLeagueResult> GetTennisResult(CriteriaBase criteria, string matchId, int bettypeId);
    }
}
