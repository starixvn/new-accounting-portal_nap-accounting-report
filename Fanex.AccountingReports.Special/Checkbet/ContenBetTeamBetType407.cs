using System.Collections.Generic;

namespace Fanex.AccountingReports.Special.Checkbet
{
    public class ContenBetTeamBetType407 : ContentBetTeam
    {
        public string ContentBetTeamBuilder(BetTeamModel model)
        {
            var team = new Dictionary<string, string>();
            team.Add("G0", "0 Goals");
            team.Add("G1", "1 Goal");
            team.Add("G2", "2 Goals");
            team.Add("G3", "3+");

            return team.ContainsKey(model.BetTeam) == true ? team[model.BetTeam] : string.Empty;
        }
    }
}
