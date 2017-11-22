using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType410 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("12", model.HomeTeam + " or " + model.AwayTeam);
            team.Add("1X", model.HomeTeam + " or Draw");
            team.Add("2X", model.AwayTeam + " or Draw");

            return team.ContainsKey(model.BetTeam) == true ? team[model.BetTeam] : string.Empty;
        }
    }
}
