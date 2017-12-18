namespace Fanex.BetList.Accounting.Services
{
    using Fanex.BetList.Accounting.Criteria;
    using Fanex.BetList.Accounting.Models.ScoreInformation;

    public interface IScoreInformationService
    {
        ScoreInformation GetScoreInfo(GetScoreInformationCriteria criteria);
    }
}