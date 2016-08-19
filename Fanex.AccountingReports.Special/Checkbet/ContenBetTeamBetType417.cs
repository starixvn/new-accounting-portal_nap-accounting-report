using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType417 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("YH", "Yes" + "/" + model.HomeTeam);
            team.Add("YD", "Yes/Draw");
            team.Add("YA", "Yes" + "/" + model.AwayTeam);
            team.Add("NH", "No" + "/" + model.HomeTeam);
            team.Add("ND", "No/Draw");
            team.Add("NA", "No" + "/" + model.AwayTeam);

            return team.ContainsKey(model.BetTeam) == true ? team[model.BetTeam] : string.Empty;
        }
    }
}
