namespace Fanex.AccountingReports.Special.Models.SpecialMatchAnalysis
{
    using System.Collections.Generic;
    
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeagueMatch : BaseModel
    {
        public LeagueMatch()
        {
            Matches = new List<Match>();
        }

        public List<Match> Matches { get; set; }
    }
}
