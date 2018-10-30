using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType418 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("YO", "Yes & Over");
            team.Add("YU", "Yes & Under");
            team.Add("NO", "No & Over");
            team.Add("NU", "No & Under");

            return team.ContainsKey(model.BetTeam) ? team[model.BetTeam] : string.Empty;
        }
    }
}