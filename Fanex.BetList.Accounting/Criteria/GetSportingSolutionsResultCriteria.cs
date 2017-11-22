namespace Fanex.BetList.Accounting.Criteria
{
    using Fanex.Data.Repository;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class GetSportingSolutionsResultCriteria
    {
        [SpParam(SqlParamName = "Matchid")]
        public int MatchId { get; set; }

        public string Resourceid { get; set; }

        public string GetSettingKey()
        {
            return "SoprtingSolutionBetlistResult";
        }

        public bool IsValid()
        {
            return true;
        }
    }
}
