using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContentBetTeamBetType412And429 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("0", "No Goal");
            team.Add("1", "1 Goal");
            team.Add("2", "2 Goals");
            team.Add("3-OVER", "3+");

            return team.ContainsKey(model.BetTeam) == true ? team[model.BetTeam] : string.Empty;
        }
    }
}
