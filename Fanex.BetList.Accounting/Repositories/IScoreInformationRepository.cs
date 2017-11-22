namespace Fanex.BetList.Accounting.Repositories
{
    using System.Collections;
    using System.Collections.Generic;
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.Result.Core.Sportsbook.DTO;
    
    public interface IScoreInformationRepository
    {
        List<Sportsbook> GetScoreInformation(GetScoreInformationCriteria criteria, ref Hashtable outputParams);

        List<dynamic> GetSportingSolutionsResult(GetSportingSolutionsResultCriteria criteria);
    }
}
