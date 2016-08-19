using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType415 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("HH", model.HomeTeam + "/" + model.HomeTeam);
            team.Add("DH", "Draw" + "/" + model.HomeTeam);
            team.Add("AH", model.AwayTeam + "/" + model.HomeTeam);
            team.Add("HA", model.HomeTeam + "/" + model.AwayTeam);
            team.Add("DA", "Draw" + "/" + model.AwayTeam);
            team.Add("AA", model.AwayTeam + "/" + model.AwayTeam);
            team.Add("NO", "None");

            return team.ContainsKey(model.BetTeam) == true ? team[model.BetTeam] : string.Empty;
        }
    }
}
