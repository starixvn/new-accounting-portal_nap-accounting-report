using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetTypeDoubleChance : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("HD", model.HomeTeam + "/" + "Draw");
            team.Add("HA", model.HomeTeam + "/" + model.AwayTeam);
            team.Add("DA", "Draw" + "/" + model.AwayTeam);

            return team.ContainsKey(model.BetTeam) == true ? team[model.BetTeam] : string.Empty;
        }
    }
}
