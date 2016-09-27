using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType426 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("H1", model.HomeTeam + " to Win by 1 Goal");
            team.Add("H2+", model.HomeTeam + " to Win by 2up Goals");
            team.Add("D", "Score Draw");
            team.Add("A1", model.AwayTeam + " to Win by 1 Goal");
            team.Add("A2+", model.AwayTeam + " to Win by 2up Goals");
            team.Add("NG", "No Goal");

            return team.ContainsKey(model.BetTeam) ? team[model.BetTeam] : string.Empty;
        }
    }
}
